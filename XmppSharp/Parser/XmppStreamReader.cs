using System.Xml;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

/// <summary>
/// Interface to abstract xmpp parser APIs based on <see cref="XmlReader" /> or that need a constant stream.
/// </summary>
public interface IXmppStreamProcessor
{
    /// <summary>
    /// Tells underlying xml reader to proceed with reading XML chars and processing XML tokens.
    /// </summary>
    /// <returns>
    /// Returns <see langword="true"/> if the parser was able to move forward and processed something; <see langword="false" /> if you were unable to read it or if an error occurred.
    /// <para>
    /// Note that this function will block until it has enough data for the reader to process the XML.
    /// </para>
    /// </returns>
    bool Advance();
}

/// <summary>
/// Interface to abstract xmpp parser APIs based on dynamic byte streams.
/// </summary>
public interface IXmppChunkedParser
{
    /// <summary>
    /// Tells the underlying parser to add bytes for XML processing.
    /// </summary>
    /// <param name="buffer">Buffer containing possible XML tokens to be processed.</param>
    /// <param name="length">Amount of bytes to process from <paramref name="buffer"/>.</param>
    void Write(byte[] buffer, int length);
}

/// <summary>
/// XMPP parser implementation based on .NET's <see cref="XmlReader" />.
/// </summary>
public class XmppStreamReader : XmppParser, IXmppStreamProcessor
{
    private Stream? _baseStream;
    private readonly bool _leaveOpen;
    private XmlReader? _reader;
    private Element? _current;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="baseStream">Stream that will be used to read the XML.</param>
    /// <param name="bufferSize">Buffer size (in characters) for reading.</param>
    /// <param name="leaveOpen">Determines whether to keep the <paramref name="baseStream"/> open when this parser is to be disposed.</param>
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
        if (_disposed)
            return false;

        if (_reader == null)
            return false;

        if (!_reader.Read())
            return false;

        switch (_reader.NodeType)
        {
            case XmlNodeType.Element:
                {
                    var elem = ElementFactory.CreateElement(_reader.Name, _reader.NamespaceURI, _current);

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
                            if (_current == null)
                                FireOnStreamElement(elem);
                            else
                                _current.AddChild(elem);
                        }
                        else
                        {
                            _current?.AddChild(elem);
                            _current = elem;
                        }
                    }
                }
                break;

            case XmlNodeType.EndElement:
                {
                    if (_current != null)
                    {
                        var parent = _current.Parent;

                        if (parent == null)
                            FireOnStreamElement(_current);

                        _current = parent;
                    }
                }
                break;

            case XmlNodeType.Comment:
                _current?.AddChild(new Comment(_reader.Value));
                break;

            case XmlNodeType.CDATA:
                _current?.AddChild(new Cdata(_reader.Value));
                break;

            case XmlNodeType.Text:
            case XmlNodeType.SignificantWhitespace:
                _current?.AddChild(new Text(_reader.Value));
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
