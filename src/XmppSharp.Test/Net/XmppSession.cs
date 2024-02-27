using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmppSharp.Entities;
using XmppSharp.Exceptions;
using XmppSharp.Protocol;
using XmppSharp.Utilities;
using XmppSharp.Xmpp;

namespace XmppSharp.Net;

public class XmppSession : IDisposable
{
	private Stream? _stream;
	private XmppServer? _server;
	private volatile XmppSessionState _sessionState;
	private volatile StreamState _streamState;
	private CancellationTokenSource? _cts;
	private ConcurrentQueue<(byte[] Buffer, Action<Exception?> Callback)>? _queue = [];
	private XmppParser? _parser;
	private volatile bool _disposed;

	public string Id { get; }
	public IPAddress RemoteAddress { get; }
	public Jid Jid { get; private set; }

	public XmppSessionState State => _sessionState;

	public XmppSession(XmppServer server, Socket socket)
	{
		_server = server;
		_stream = new NetworkStream(socket, true);
		_sessionState = XmppSessionState.None;

		Id = _server.GenerateSessionId();
		Jid = new Jid { User = "unknown", Server = _server._config.Hostname };
		RemoteAddress = (socket.RemoteEndPoint as IPEndPoint)!.Address;

		Debug.Assert(RemoteAddress != null);
	}

	static readonly string XmppEndTag = "</stream:stream>";

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		_tlsCert?.Dispose();
		_tlsCert = default;

		_sessionState = XmppSessionState.None;
		_streamState = StreamState.None;

		if (_queue != null)
		{
			if (!_queue.IsEmpty)
			{
				var exception = new ObjectDisposedException(GetType().FullName);

				while (_queue.TryDequeue(out var item))
					item.Callback?.Invoke(exception);
			}

			_queue = null;
		}

		_stream?.Dispose();
		_stream = null;

		_server?.UnregisterSession(this);
		_server = null;

		GC.SuppressFinalize(this);
	}

	#region Initialization Methods

	public async Task InitializeAsync(CancellationToken token = default)
	{
		await Task.Yield();

		_cts = CancellationTokenSource.CreateLinkedTokenSource(token);
		_streamState = StreamState.ReadWrite;
		await _server!.FireOnClientConnected(this);

		_ = Task.Run(BeginReceive, token);
		_ = Task.Run(BeginSend, token);
	}

	internal void InitParser(bool leaveSocketOpen = true)
	{
		if (_parser != null)
		{
			_parser.OnStreamStart -= OnXmppStartTag;
			_parser.OnStreamElement -= OnXmppElement;
			_parser.OnStreamEnd -= OnXmppEndTag;
			_parser.Dispose();
		}

		if (_disposed)
			return;

		_parser = new XmppParser(_stream!, Encoding.UTF8, _server!._config.ParserBufferSize, leaveSocketOpen);
		_parser.OnStreamStart += OnXmppStartTag;
		_parser.OnStreamElement += OnXmppElement;
		_parser.OnStreamEnd += OnXmppEndTag;
	}

	#endregion

	public async Task DisconnectAsync(StreamErrorCondition? reason = default, string? description = default)
	{
		var sb = new StringBuilder();

		if (reason.TryUnwrap(out var self))
			sb.Append(self.CreateElement(description).ToString(false));

		sb.Append(XmppEndTag);

		await SendAsync(sb.ToString());

		CheckDisposed();
	}

	async Task OnXmppEndTag()
	{
		LogXml(this, StreamState.Read, XmppEndTag);
		await DisconnectAsync();
	}

	#region Read Thread

	async Task BeginReceive()
	{
		StreamErrorCondition? condition = default;
		string? description = default;

		try
		{
			InitParser();

			while (_cts != null && !_cts.IsCancellationRequested)
			{
				await Task.Delay(1);

				if (_disposed)
					return;

				if (!_streamState.HasFlag(StreamState.Read))
					continue;

				await _parser!.UpdateAsync();
			}
		}
		catch (JabberStreamException ex)
		{
			condition = ex.Error;
			description = ex.Message;
		}
		catch (XmlException)
		{
			condition = StreamErrorCondition.NotWellFormed;
		}
		catch (Exception)
		{
			condition = StreamErrorCondition.InternalServerError;
		}

		await DisconnectAsync(condition, description);
	}

	#endregion

	#region Write Thread

	async Task BeginSend()
	{
		while (_cts != null && !_cts.IsCancellationRequested)
		{
			await Task.Delay(1);

			if (_disposed)
				return;

			if (!_streamState.HasFlag(StreamState.Write))
				continue;

			if (_queue == null || _stream == null)
				break;

			if (!_queue.IsEmpty)
			{
				while (_queue != null && _queue.TryDequeue(out var entry))
				{
					var (buffer, callback) = entry;

					Exception? exception = default;

					try
					{
						await _stream!.WriteAsync(buffer, _cts.Token);
					}
					catch (Exception e)
					{
						exception = e;
					}
					finally
					{
						callback?.Invoke(exception);
					}
				}
			}
		}

		CheckDisposed();
	}

	#endregion

	void CheckDisposed()
	{
		if (!_disposed)
		{
			if (_server != null)
				_ = _server.FireOnClientDisconnected(this);

			Dispose();
		}
	}

	async Task OnXmppStartTag(XElement e)
	{
		LogXml(this, StreamState.Read, e.StartTag());

		StreamErrorCondition? condition = null;
		string targetHostname = _server!._config.Hostname;

		if (!e.HasAttribute("to"))
			condition = StreamErrorCondition.ImproperAddressing;
		else if (targetHostname != e.GetAttribute("to"))
			condition = StreamErrorCondition.HostUnknown;
		else if (e.GetDefaultNamespace() != Namespace.Client.Get())
			condition = StreamErrorCondition.UnsupportedStanzaType;

		e.RemoveAttribute("to");
		e.SetAttribute("from", targetHostname);
		e.SetAttribute("id", Id);
		await SendAsync(e.StartTag());

		if (condition.TryUnwrap(out var v))
		{
			await DisconnectAsync(v);
			return;
		}

		var features = Namespace.Stream.CreateElement("features");
		{
			if (!_sessionState.HasFlag(XmppSessionState.Authenticated))
			{
				if (!_sessionState.HasFlag(XmppSessionState.TlsStarted)
					&& _server._config.Tls.Policy > TlsPolicy.None)
				{
					var tls = features.C(Namespace.Tls.CreateElement("starttls"));

					if (_server._config.Tls.Policy == TlsPolicy.Required)
						tls.C("required");
				}

				var m = features.C(Namespace.Sasl.CreateElement("mechanisms"));
				{
					foreach (var mechanismName in _server._config.SupportedMechanisms)
						m.C("mechanism", text: mechanismName);
				}
			}
			else
			{
				if (!_sessionState.HasFlag(XmppSessionState.ResourceBinded))
					features.C(Namespace.Bind.CreateElement("bind"));

				if (!_sessionState.HasFlag(XmppSessionState.SessionStarted))
					features.C(Namespace.Session.CreateElement("session"));
			}
		}

		await SendAsync(features);
	}

	#region Xml Logging

	public event Action<string?, Exception?>? OnWriteXml;
	public event Action<string?>? OnReadXml;

	static void LogXml(XmppSession session, StreamState state, string xml, Exception? exception = default)
	{
		if (state == StreamState.Write)
			session.OnWriteXml?.Invoke(xml, exception);
		else
			session.OnReadXml?.Invoke(xml);
	}

	#endregion

	#region Async Send Methods

	internal Task SendAsync(string xml)
	{
		if (_disposed)
			return Task.CompletedTask;

		Debug.Assert(_queue != null);

		var tcs = new TaskCompletionSource();

		var callback = (Exception? ex) =>
		{
			_ = ex == null
				? tcs.TrySetResult()
				: tcs.TrySetException(ex);

			LogXml(this, StreamState.Write, xml);
		};

		var buffer = xml.GetBytes();
		_queue.Enqueue((buffer, callback));
		return tcs.Task;
	}

	public Task SendAsync(XElement element)
	{
		if (_disposed)
			return Task.CompletedTask;

		Debug.Assert(_queue != null);

		var tcs = new TaskCompletionSource();

		var callback = (Exception? ex) =>
		{
			_ = ex == null
				? tcs.TrySetResult()
				: tcs.TrySetException(ex);

			LogXml(this, StreamState.Write, element.ToString(true), ex);
		};

		var buffer = element.GetBytes();
		_queue.Enqueue((buffer, callback));

		return tcs.Task;
	}

	#endregion

	#region Sync Send Methods

	internal void Send(string xml)
	{
		if (_disposed)
			return;

		Debug.Assert(_queue != null);

		var callback = (Exception? ex) =>
		{
			LogXml(this, StreamState.Write, xml, ex);
		};

		var buffer = xml.GetBytes();
		_queue.Enqueue((buffer, callback));
	}

	internal void Send(XElement element)
	{
		if (_disposed)
			return;

		Debug.Assert(_queue != null);

		var callback = (Exception? ex) =>
		{
			LogXml(this, StreamState.Write, element.ToString(true), ex);
		};

		var buffer = element.GetBytes();
		_queue.Enqueue((buffer, callback));
	}

	#endregion

	public event ParameterizedAsyncEventHandler<XElement>? OnElement;

	async Task OnXmppElement(XElement e)
	{
		await Task.Yield();
		LogXml(this, StreamState.Read, e.ToString(true));

		if (!_sessionState.HasFlag(XmppSessionState.Authenticated))
		{
			if (e.Is("starttls", Namespace.Tls))
			{
				if (_sessionState.HasFlag(XmppSessionState.TlsStarted))
					await DisconnectAsync(StreamErrorCondition.InvalidXml);
				else
				{
					_streamState &= ~StreamState.Read;
					await SendAsync(Namespace.Tls.CreateElement("proceed"));
					await InitializeTlsAsync();
				}
			}
			else if (e.Is("auth", Namespace.Sasl))
			{
				// TODO: Handle auth later.
				Jid = Jid with { User = Guid.NewGuid().ToString("D") };
				_sessionState |= XmppSessionState.Authenticated;
				await SendAsync($"<success xmlns='{e.GetDefaultNamespace()}' />");
			}
		}
		else
		{
			if (e.Is("presence") || e.Is("message"))
			{
				if (!_sessionState.HasFlag(XmppSessionState.SessionStarted))
				{
					e.SwitchDirection();
					e.Add(StanzaErrorCondition.UnexpectedRequest.CreateElement(e.GetDefaultNamespace(), StanzaErrorType.Cancel));
					await SendAsync(e);
				}
				else
				{
					// Handle presence, message tags
				}
			}

			if (e.Is("iq"))
			{
				var child = e.FirstChild();

				if (child.Is("bind", Namespace.Bind))
				{
					StanzaErrorCondition? condition = default;

					var resource = child.Descendants().First(x => x.TagName() == "resource")?.Value;

					if (resource == null)
						condition = StanzaErrorCondition.BadRequest;
					else
					{
						var fullJid = Jid with { Resource = resource };

						if (_server!.GetSession(x => x.Jid == fullJid) != null)
							condition = StanzaErrorCondition.Conflict;
						else
						{
							Jid = Jid with { Resource = resource };
							_sessionState |= XmppSessionState.ResourceBinded;
						}
					}

					e.SwitchDirection();
					e.SetAttribute("type", condition.HasValue ? "error" : "result");

					if (condition.TryUnwrap(out var v))
					{
						e.Add(v.CreateElement(e.GetDefaultNamespace()));
						await SendAsync(e);
						await DisconnectAsync();
					}
					else
					{
						child.Value = string.Empty;
						child.C("jid", Jid.ToString());
						_sessionState |= XmppSessionState.ResourceBinded;
						await SendAsync(e);
					}
				}
				else if (child.Is("session", Namespace.Session))
				{
					if (!_sessionState.HasFlag(XmppSessionState.ResourceBinded))
					{
						e.SwitchDirection();
						e.SetAttribute("type", "error");
						e.Add(StanzaErrorCondition.UnexpectedRequest.CreateElement(e.GetDefaultNamespace(), StanzaErrorType.Modify));
						await SendAsync(e);
					}

					// TODO: handle session.
					e.SwitchDirection();
					e.SetAttribute("type", "result");
					_sessionState |= XmppSessionState.SessionStarted;
					await SendAsync(e);
				}
				else
				{
					if (!_sessionState.HasFlag(XmppSessionState.ResourceBinded | XmppSessionState.SessionStarted))
					{
						e.SwitchDirection();
						e.SetAttribute("type", "error");
						e.Add(StanzaErrorCondition.NotAllowed.CreateElement(e.GetDefaultNamespace(), StanzaErrorType.Cancel));
						await SendAsync(e);
					}
				}
			}
		}

		goto _fallback;

	//return;

	_fallback:
		await OnElement.InvokeAsync(e);
		return;
	}

	private X509Certificate2 _tlsCert;

	async Task InitializeTlsAsync()
	{
		{
			// flush write queue before we start tls handshake.

			var tcs = new TaskCompletionSource();

			_queue!.Enqueue(([], ex => _ = ex != null
				? tcs.TrySetException(ex)
				: tcs.TrySetResult()));

			await tcs.Task;
		}

		//_parser.Reset();
		_streamState = StreamState.None;
		_parser?.Dispose();
		_tlsCert = await _server!.CertificateProvider!.ProvideAsync(_cts!.Token)!;

		if (_tlsCert == null)
		{
			Dispose();
			return;
		}

		try
		{
			var newStream = new SslStream(_stream!, false);
			await newStream.AuthenticateAsServerAsync(_tlsCert, false, _server!._config.Tls.EnabledProtocols, true);
			_stream = newStream;

			_sessionState |= XmppSessionState.TlsStarted;
			InitParser(false);
			_streamState = StreamState.ReadWrite;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			Dispose();
			return;
		}
	}
}
