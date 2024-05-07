using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Expat;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parsers;

/// <summary>
/// An enhanced XMPP parser built using Expat library.
/// </summary>
public partial class ExpatXmppParser : BaseXmppParser
{
	private Parser _parser;
	private Element _currentElem;
	private XmlNamespaceManager _nsStack;
	private NameTable _xmlNames;

	void AddNamespacesToScope(IReadOnlyDictionary<string, string> attrs)
	{
		foreach (var (key, value) in attrs)
		{
			if (key == "xmlns")
				this._nsStack.AddNamespace(string.Empty, value);
			else if (key.StartsWith("xmlns:"))
			{
				var prefix = key[(key.IndexOf(':') + 1)..];
				this._nsStack.AddNamespace(prefix, value);
			}
		}
	}

	public ExpatXmppParser(ExpatEncodingType encoding = ExpatEncodingType.Utf8)
	{
		this._nsStack = new(this._xmlNames = new NameTable());

		this._parser = new Parser(encoding);

		this._parser.OnElementStart += e =>
		{
			this._nsStack.PushScope();

			AddNamespacesToScope(e.Attributes);

			var qname = Xml.ExtractQualifiedName(e.Name);

			var ns = this._nsStack.LookupNamespace(qname.HasPrefix ? qname.Prefix : string.Empty);

			if (e.Name is "iq" or "message" or "presence") // work-around
				ns ??= Namespace.Client;

			var element = ElementFactory.Create(e.Name, ns);

			//foreach (var (key, value) in _nsStack.GetNamespacesInScope(XmlNamespaceScope.Local))
			//{
			//	var att = string.IsNullOrWhiteSpace(key) ? "xmlns" : $"xmlns:{key}";
			//	element.SetAttribute(att, value);
			//}

			foreach (var (key, value) in e.Attributes)
				element.SetAttribute(key, value);

			if (e.Name == "stream:stream")
				AsyncHelper.RunSync(() => FireStreamStart(element as StreamStream));
			else
			{
				_currentElem?.AddChild(element);
				_currentElem = element;
			}
		};

		this._parser.OnElementEnd += e =>
		{
			this._nsStack.PopScope();

			if (e.Value == "stream:stream")
				AsyncHelper.RunSync(() => FireStreamEnd());
			else
			{
				var parent = _currentElem.Parent;

				if (parent == null)
					AsyncHelper.RunSync(() => FireStreamElement(_currentElem));
				else
				{
					if (e.Value != _currentElem.TagName)
					{
						var ex = new JabberStreamException(StreamErrorCondition.InvalidXml, "Parent end tag mismatch.");
						ex.Data.Add("Actual", e.Value);
						ex.Data.Add("Expected", _currentElem.TagName);
						throw ex;
					}
				}

				_currentElem = parent;
			}
		};

		this._parser.OnText += e =>
		{
			if (_currentElem != null)
			{
				var trimWS = _currentElem.GetAttribute("xml:space") != "preserve";

				// skip whitespace if not explicit declared.
				if (string.IsNullOrWhiteSpace(e.Value) && trimWS)
					return;

				var val = e.Value;

				if (trimWS) // same for trailing whitespace
					val = TrimWhitespace(val);

				if (_currentElem.LastNode is Text text)
					text.Value += val;
				else
					_currentElem.AddChild(new Text(val));
			}
		};

		this._parser.OnCdata += e =>
		{
			this._currentElem?.AddChild(new Cdata(e.Value));
		};

		this._parser.OnComment += e =>
		{
			this._currentElem?.AddChild(new Comment(e.Value));
		};
	}

	[GeneratedRegex("\n")]
	protected static partial Regex NewLineRegex();

	[GeneratedRegex(@"\s+")]
	protected static partial Regex ContiguousSpaceRegex();

	static string TrimWhitespace(string str)
	{
		if (string.IsNullOrEmpty(str))
			return string.Empty;

		str = NewLineRegex().Replace(str, string.Empty);
		str = str.Replace("\t", " ");
		str = str.Trim();
		str = ContiguousSpaceRegex().Replace(str, " ");

		return str;
	}

	public void Reset()
	{
		this.EnsureNotDisposed();

		while (this._nsStack.PopScope())
			;

		// reset namespace stack.
		this._nsStack = new(this._xmlNames);

		// reset the parser
		this._parser.Reset();
	}

	public void Write(byte[] buffer, int offset, int length, bool isFinalBlock = false)
	{
		this.EnsureNotDisposed();

		byte[] temp;

		try
		{
			temp = GC.AllocateUninitializedArray<byte>(length, true);
			Buffer.BlockCopy(buffer, offset, temp, 0, length);
			this._parser.Feed(temp, length, isFinalBlock);
		}
		finally
		{
			temp = null;
		}
	}

	public void Write(byte[] buffer, int length, bool isFinalBlock = false)
	{
		this.EnsureNotDisposed();
		this._parser.Feed(buffer, length, isFinalBlock);
	}

	public void Write(byte[] buffer, bool isFinalBlock = false)
	{
		this.EnsureNotDisposed();
		this._parser.Feed(buffer, buffer.Length, isFinalBlock);
	}

	protected override void Disposing()
	{
		this._xmlNames = null;

		while (this._nsStack.PopScope())
			;

		this._nsStack = null;

		this._parser?.Dispose();
		this._parser = null;
	}
}
