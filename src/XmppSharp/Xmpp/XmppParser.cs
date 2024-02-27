using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmppSharp.Exceptions;
using XmppSharp.Protocol;

namespace XmppSharp.Xmpp;

public class XmppParser : IDisposable
{
	private XmlReader? _reader;
	private volatile bool _disposed;

	public event ParameterizedAsyncEventHandler<XElement>? OnStreamStart;
	public event ParameterizedAsyncEventHandler<XElement>? OnStreamElement;
	public event AsyncEventHandler? OnStreamEnd;

	public XmppParser(Stream stream, Encoding? encoding = default, int bufferSize = 1024, bool leaveOpen = true)
	{
		_reader = XmlReader.Create(new StreamReader(stream, encoding ?? Encoding.UTF8, false, bufferSize, leaveOpen), new()
		{
			Async = true,
			CloseInput = true,
			IgnoreWhitespace = true,
			XmlResolver = XmlResolver.ThrowingResolver,
			IgnoreProcessingInstructions = true,
			IgnoreComments = true,
			DtdProcessing = DtdProcessing.Prohibit, // https://en.wikipedia.org/wiki/Billion_laughs_attack
		});
	}

	private XElement? _current;

	void CheckDisposed()
		=> ObjectDisposedException.ThrowIf(_disposed, this);

	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	int GetLine(bool isLineNum)
	{
		if (_disposed)
			return 0;

		if (_reader is not IXmlLineInfo info)
			return 0;

		if (info.HasLineInfo())
			return isLineNum ? info.LineNumber : info.LinePosition;

		return 0;
	}

	public int LineNumber => GetLine(false);
	public int LinePosition => GetLine(true);

	protected virtual XElement CreateElement()
	{
		XName elementName;

		{
			var localName = _reader!.LocalName;
			var namespaceUri = _reader!.NamespaceURI;

			if (namespaceUri != null)
				elementName = XName.Get(localName, namespaceUri);
			else
				elementName = localName;
		}

		var element = new XElement(elementName);

		if (_reader.HasAttributes)
		{
			while (_reader.MoveToNextAttribute())
			{
				if (_reader.Name.Contains(':'))
				{
					var ofs = _reader.Name.IndexOf(':');
					var prefix = _reader.Name[0..ofs];
					var localName = _reader.Name[(ofs + 1)..];

					XNamespace ns = prefix switch
					{
						"xml" => XNamespace.Xml,
						"xmlns" => XNamespace.Xmlns,
						_ => _reader.LookupNamespace(prefix)!
					};

					element.SetAttributeValue(ns + localName, _reader.Value);
				}
				else
				{
					element.SetAttributeValue(_reader.Name, _reader.Value);
				}
			}

			_reader.MoveToElement();
		}

		return element;
	}

	public async Task UpdateAsync()
	{
		CheckDisposed();

		var result = await _reader!.ReadAsync();

		if (!result)
			throw new JabberStreamException(StreamErrorCondition.HostGone);

		switch (_reader.NodeType)
		{
			case XmlNodeType.Element:
				{
					var element = CreateElement();

					if (element.TagName() == "stream:stream")
						await OnStreamStart.InvokeAsync(element);
					else
					{
						if (_reader.IsEmptyElement)
						{
							if (_current != null)
								_current.Add(element);
							else
								await OnStreamElement.InvokeAsync(element);
						}
						else
						{
							_current?.Add(element);
							_current = element;
						}
					}
				}
				break;

			case XmlNodeType.EndElement:
				{
					if (_reader.Name == "stream:stream")
						await OnStreamEnd.InvokeAsync();
					else
					{
						Debug.Assert(_current != null);

						if (_reader.Name != _current.TagName())
							throw new JabberStreamException(StreamErrorCondition.UnsupportedStanzaType);
						else
						{
							var parent = _current.Parent;

							if (parent == null)
								await OnStreamElement.InvokeAsync(_current);

							_current = parent;
						}
					}
				}
				break;

			case XmlNodeType.SignificantWhitespace:
			case XmlNodeType.Text:
				{
					Debug.Assert(_current != null);

					if (_current.LastNode is XText text)
						text.Value += _reader.Value;
					else
						_current.Add(new XText(_reader.Value));
				}
				break;

			case XmlNodeType.XmlDeclaration:
				// skip xml decl
				break;

			default:
				throw new JabberStreamException(StreamErrorCondition.RestrictedXml);
		}
	}


	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		_current = null;
		_reader?.Dispose();
		_reader = null;

		GC.SuppressFinalize(this);
	}
}
