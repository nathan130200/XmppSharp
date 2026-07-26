using System.Net;
using System.Net.Sockets;
using System.Xml;
using XmppSharp;
using XmppSharp.Dom;
using XmppSharp.Parser;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Component;
using XmppSharp.Serialization;

class Component : IDisposable
{
	public Jid Jid { get; private set; } = default!;

	Socket? _socket;
	Stream? _stream;
	StreamParser? _parser;

	volatile bool _disposed;

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;

			_stream?.Dispose();

			_stream = null;

			_parser?.Dispose();

			_parser = null;

			_socket = null;
		}
	}

	public void Connect(EndPoint endpoint, string hostname, string password)
	{
		var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

		try
		{
			socket.Connect(endpoint);

			_socket = socket;

			_stream = new NetworkStream(socket, true);

			_componentHostname = hostname;

			_componentPassword = password;

			Setup();
		}
		catch
		{

			socket.Dispose();

			throw;
		}
	}

	bool _authenticated;

	string? _componentHostname, _componentPassword;

	readonly SemaphoreSlim _semaphore = new(1, 1);

	public void Disconnect()
	{
		if (_disposed)
			return;

		_semaphore.Wait();

		try
		{
			_socket!.Shutdown(SocketShutdown.Receive);

			_stream!.Write(StringHelper.ToBytes(c_XmppStreamEnd));

			OnWriteXml?.Invoke(c_XmppStreamEnd);

			Dispose();
		}
		finally
		{
			_semaphore.Release();
		}
	}


	public void Send(Element e)
	{
		if (_disposed)
			return;

		_semaphore.Wait();

		try
		{
			var xml = e.ToString(false);

			_stream!.Write(StringHelper.ToBytes(xml));

			OnWriteXml?.Invoke(xml);
		}
		finally
		{
			_semaphore.Release();
		}
	}

	public event Action<Exception>? OnError;

	public event Action<string>? OnReadXml;

	public event Action<string>? OnWriteXml;

	public event Action<Element>? OnElement;

	const string c_XmppStreamEnd = "</stream:stream>";

	void Setup()
	{
		_parser = new StreamParser();

		_parser.OnStreamStart += e =>
		{
			OnReadXml?.Invoke(e.ToString());

			Send(new Handshake(e.Id!, _componentPassword!));
		};

		_parser.OnStreamElement += e =>
		{
			OnReadXml?.Invoke(e.ToString(false));

			if (!_authenticated)
			{
				if (e is Handshake)
				{
					_authenticated = true;
					_componentPassword = null;
					Jid = _componentHostname!;
				}
				else if (e is StreamError se)
				{
					throw new XmlException((se.Text ?? "Authentication failed.") + " (code: " + (int)se.Condition + ")")
					{
						Data =
						{
							["Payload"] = se
						}
					};
				}
			}
			else
			{
				var handler = OnElement;

				if (handler == null) return;

				_ = Task.Run(() => handler(e))
					.ContinueWith(task => Console.WriteLine(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
			}
		};

		_parser.OnStreamEnd += () =>
		{
			OnReadXml?.Invoke(c_XmppStreamEnd);

			Dispose();
		};

		_ = Task.Run(async () =>
		{
			using var buf = new RentedArray<byte>(4096);

			try
			{
				while (!_disposed)
				{
					int len = await _stream!.ReadAsync(buf);

					if (len == 0) break;

					_parser!.Parse(buf.Span[..len]);
				}
			}
			catch (Exception ex)
			{
				OnError?.Invoke(ex);
			}
			finally
			{
				Dispose();
			}
		});

		InitStream();
	}

	void InitStream()
	{
		Stanza.DefaultElementNamespace = Namespaces.Component;

		Send(new StreamStream
		{
			To = _componentHostname,
		});
	}
}