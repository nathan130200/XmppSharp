using System.Text.RegularExpressions;
using System.Xml;
using Expat;
using Expat.Native;
using XmppSharp.Dom;
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
	private XmlNamespaceManager _namespaces;
	private NameTable _xmlNameTable;

	void AddNamespacesToScope(IReadOnlyDictionary<string, string> attrs)
	{
		foreach (var (key, value) in attrs)
		{
			if (key == "xmlns")
				this._namespaces.AddNamespace(string.Empty, value);
			else if (key.StartsWith("xmlns:"))
			{
				var prefix = key[(key.IndexOf(':') + 1)..];
				this._namespaces.AddNamespace(prefix, value);
			}
		}
	}

	public ExpatXmppParser(EncodingType encoding = EncodingType.UTF8)
	{
		this._namespaces = new(this._xmlNameTable = new NameTable());

		this._parser = new ExpatParser(encoding);

		this._parser.OnElementStart += (name, attributes) =>
		{
			this._namespaces.PushScope();

			AddNamespacesToScope(attributes);

			var qname = Xml.ExtractQualifiedName(name);

			var ns = this._namespaces.LookupNamespace(qname.HasPrefix ? qname.Prefix : string.Empty);

			if (name is "iq" or "message" or "presence") // work-around
				ns ??= Namespaces.Client;

			var element = ElementFactory.Create(name, ns);

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
			this._namespaces.PopScope();

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

		this._parser.OnText += (text) =>
		{
			var trimWhitespace = !(_currentElem.GetAttribute("xml:space") == "preserve");

			if (trimWhitespace)
				text = TrimAllWhitespace(text);

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

	[GeneratedRegex("\n")]
	protected static partial Regex NewLineRegex();

	[GeneratedRegex(@"\s+")]
	protected static partial Regex ContiguousSpaceRegex();

	static string TrimAllWhitespace(string str)
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
		this.ThrowIfDisposed();

		this._namespaces = new(this._xmlNameTable);
		this._parser.Reset();
	}

	public void Write(byte[] buffer, int count, bool isFinal = false)
	{
		this.ThrowIfDisposed();
		this._parser.Write(buffer, count, isFinal);
	}

	protected override void Release()
	{
		this._xmlNameTable = null;

		while (this._namespaces.PopScope())
			;

		this._namespaces = null;

		this._parser?.Dispose();
		this._parser = null;
	}
}
