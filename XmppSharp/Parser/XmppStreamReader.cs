using System.Xml;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public class XmppStreamReader : XmppParser
{
    private Stream? _baseStream;
    private readonly bool _leaveOpen;
    private XmlReader? _reader;
    private Element? current;

    public XmppStreamReader(Stream baseStream, int bufferSize = -1, bool leaveOpen = true)
    {
        _baseStream = baseStream;
        _leaveOpen = leaveOpen;

        _reader = XmlReader.Create(new StreamReader(_baseStream, bufferSize: bufferSize, leaveOpen: true), new XmlReaderSettings()
        {
            CloseInput = true,
            CheckCharacters = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
#if NET6_0
            XmlResolver = Xml.ThrowingResolver
#else
            XmlResolver = XmlResolver.ThrowingResolver
#endif
        });
    }

    public virtual bool Advance()
    {
        if (Disposed)
            return false;

        if (_reader == null)
            return false;

        if (!_reader.Read())
            return false;

        switch (_reader.NodeType)
        {
            case XmlNodeType.Element:
                {
                    var elem = ElementFactory.CreateElement(_reader.Name, _reader.NamespaceURI);

                    if (_reader.HasAttributes)
                    {
                        while (_reader.MoveToNextAttribute())
                            elem.SetAttribute(_reader.Name, _reader.Value);
                    }

                    _reader.MoveToElement();

                    if (elem is StreamStream ss)
                        FireOnStreamStart(ss);
                    else
                    {
                        if (_reader.IsEmptyElement)
                        {
                            if (current == null)
                                FireOnStreamElement(elem);
                            else
                                current.AddChild(elem);
                        }
                        else
                        {
                            current?.AddChild(elem);
                            current = elem;
                        }
                    }
                }
                break;

            case XmlNodeType.EndElement:
                {
                    if (current != null)
                    {
                        var parent = current.Parent;

                        if (parent == null)
                            FireOnStreamElement(current);

                        current = parent;
                    }
                }
                break;

            case XmlNodeType.Comment:
                current?.AddChild(new Comment(_reader.Value));
                break;

            case XmlNodeType.CDATA:
                current?.AddChild(new Cdata(_reader.Value));
                break;

            case XmlNodeType.Text:
            case XmlNodeType.SignificantWhitespace:
                current?.AddChild(new Text(_reader.Value));
                break;
        }

        return true;
    }

    protected override void DisposeCore()
    {
        try
        {
            _reader?.Dispose();
            _reader = null;
        }
        catch
        {
            // skip some XML errors (eg: unclosed tags)
        }

        if (!_leaveOpen)
        {
            _baseStream?.Dispose();
            _baseStream = null;
        }
    }
}
