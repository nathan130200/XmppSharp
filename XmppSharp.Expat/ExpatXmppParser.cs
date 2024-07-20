using System.Text.RegularExpressions;
using System.Xml;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Expat;
using XmppSharp.Expat.Native;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public partial class ExpatXmppParser : IDisposable
{
	private ExpatParser _parser;
	private Element _currentElem;
	private XmlNamespaceManager _nsStack;
	private NameTable _nameTable;

	public event Action<StreamStream> OnStreamStart;
	public event Action<Element> OnStreamElement;
	public event Action OnStreamEnd;

	public ExpatXmppParser(EncodingType encoding = EncodingType.UTF8)
	{
		this._nsStack = new(this._nameTable = new NameTable());

		this._parser = new ExpatParser(encoding);

		this._parser.OnElementStart += (name, attributes) =>
		{
			this._nsStack.PushScope();

			foreach (var (key, value) in attributes)
			{
				if (key == "xmlns")
					this._nsStack.AddNamespace(string.Empty, value);
				else if (key.StartsWith("xmlns:"))
				{
					var prefix = key[(key.IndexOf(':') + 1)..];
					this._nsStack.AddNamespace(prefix, value);
				}
			}

			var qname = Xml.ExtractQualifiedName(name);

			var ns = this._nsStack.LookupNamespace(qname.HasPrefix ? qname.Prefix : string.Empty);

			if (name is "iq" or "message" or "presence") // work-around
				ns ??= Namespaces.Client;

			var element = ElementFactory.Create(name, ns);

			foreach (var (key, value) in attributes)
				element.SetAttribute(key, value);

			if (name == "stream:stream")
				OnStreamStart?.Invoke(element as StreamStream);
			else
			{
				_currentElem?.AddChild(element);
				_currentElem = element;
			}
		};

		this._parser.OnElementEnd += (name) =>
		{
			this._nsStack.PopScope();

			if (name == "stream:stream")
				OnStreamEnd?.Invoke();
			else
			{
				var parent = _currentElem.Parent;

				if (parent == null)
					this.OnStreamElement?.Invoke(_currentElem);
				else
				{
					if (name != _currentElem.TagName)
					{
						var ex = new JabberStreamException(StreamErrorCondition.InvalidXml, "Parent end tag mismatch.");
						ex.Data.Add("Actual", name);
						ex.Data.Add("Expected", _currentElem.TagName);
						throw ex;
					}
				}

				_currentElem = parent;
			}
		};

		this._parser.OnText += (text) =>
		{
			if (_currentElem == null)
				return;

			var trimWhitespace = _currentElem.GetAttribute("xml:space") != "preserve";

			if (trimWhitespace)
				text = text.TrimWhitespaces();

			if (_currentElem.LastNode is Text node)
				node.Value += text;
			else
				_currentElem.AddChild(new Text(text));
		};

		this._parser.OnCdata += value =>
		{
			this._currentElem?.AddChild(new Cdata(value));
		};

		this._parser.OnComment += value =>
		{
			this._currentElem?.AddChild(new Comment(value));
		};
	}

	public void Reset()
	{
		this.ThrowIfDisposed();

		this._nsStack = new(this._nameTable);
		this._parser.Reset();
	}

	public void Write(byte[] buffer, int count, bool isFinal = false)
	{
		this.ThrowIfDisposed();
		this._parser.Write(buffer, count, isFinal);
	}

	public void WriteInplace(byte[] buffer, int count, bool isFinal = false)
	{
		this.ThrowIfDisposed();
		this._parser.WriteInplace(buffer, count, isFinal);
	}

	protected volatile bool _disposed;

	protected void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		this._nsStack = null;

		while (this._nsStack.PopScope())
			;

		this._nameTable = null;
		this._parser?.Dispose();
		this._parser = null;

		GC.SuppressFinalize(this);
	}
}
