using System.Text.RegularExpressions;
using System.Xml;
using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

/// <summary>
/// An enhanced XMPP parser built using Expat library.
/// </summary>
public partial class ExpatXmppParser : BaseXmppParser
{
	private ExpatParser _parser;
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

	public ExpatXmppParser(EncodingType encoding = EncodingType.UTF8)
	{
		this._nsStack = new(this._xmlNames = new NameTable());

		this._parser = new ExpatParser(encoding);

		this._parser.OnElementStart += (name, attributes) =>
		{
			this._nsStack.PushScope();

			AddNamespacesToScope(attributes);

			var qname = Xml.ExtractQualifiedName(name);

			var ns = this._nsStack.LookupNamespace(qname.HasPrefix ? qname.Prefix : string.Empty);

			if (name is "iq" or "message" or "presence") // work-around
				ns ??= Namespace.Client;

			var element = ElementFactory.Create(name, ns);

			//foreach (var (key, value) in _nsStack.GetNamespacesInScope(XmlNamespaceScope.Local))
			//{
			//	var att = string.IsNullOrWhiteSpace(key) ? "xmlns" : $"xmlns:{key}";
			//	element.SetAttribute(att, value);
			//}

			foreach (var (key, value) in attributes)
				element.SetAttribute(key, value);

			if (name == "stream:stream")
				AsyncHelper.RunSync(() => FireStreamStart(element as StreamStream));
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
				AsyncHelper.RunSync(() => FireStreamEnd());
			else
			{
				var parent = _currentElem.Parent;

				if (parent == null)
					AsyncHelper.RunSync(() => FireStreamElement(_currentElem));
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

		this._parser.OnText += (type, text) =>
		{
			if (_currentElem == null)
				return;

			if (type == ContentNodeType.Text)
			{
				var trimWhitespace = _currentElem.GetAttribute("xml:space") != "preserve";

				if (trimWhitespace && text.All(XmlConvert.IsWhitespaceChar))
					return;

				if (trimWhitespace) // same for trailing whitespace
					text = TrimWhitespace(text);

				if (_currentElem.LastNode is Text node)
					node.Value += text;
				else
					_currentElem.AddChild(new Text(text));
			}
			else if (type == ContentNodeType.Cdata)
			{
				this._currentElem.AddChild(new Cdata(text));
			}
			else if (type == ContentNodeType.Comment)
			{
				this._currentElem.AddChild(new Comment(text));
			}
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

	public void Write(byte[] buffer, int length, bool isFinalBlock = false)
	{
		this.EnsureNotDisposed();
		//this._parser.WriteBuffer(buffer, length, isFinalBlock);
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
