using Expat;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

/// <summary>
/// XMPP parser implementation based on <see cref="ExpatParser"/>.
/// </summary>
public class ExpatXmppParser : XmppParser
{
    private ExpatParser _xmlParser;
    private Element? _current;
    private NamespaceStack? _namespaces;
    private volatile bool _isStreamOpen;

    public ExpatParser XmlParser => _xmlParser;

    public ExpatXmppParser(ExpatEncoding encoding, bool strict = true)
    {
        _namespaces = new();

        _xmlParser = new ExpatParser(encoding, strict);

        _xmlParser.OnStartElement += (name, attrs) =>
        {
            _namespaces.PushScope();

            XmppName tagName = name;

            foreach (var (key, value) in attrs)
            {
                XmppName attrName = key;

                if (attrName == "xmlns")
                    _namespaces.AddNamespace(string.Empty, value);
                else if (attrName.Prefix == "xmlns")
                    _namespaces.AddNamespace(attrName.LocalName, value);
            }

            var element = ElementFactory.CreateElement(tagName, _namespaces.LookupNamespace(tagName.Prefix), _current);

            foreach (var (key, value) in attrs)
                element.SetAttribute(key, value);

            if (element is StreamStream start)
            {
                if (!_isStreamOpen)
                {
                    _isStreamOpen = true;
                    FireOnStreamStart(start);
                }
            }
            else
            {
                _current?.AddChild(element);
                _current = element;
            }
        };

        _xmlParser.OnEndElement += name =>
        {
            if (name == "stream:stream")
            {
                if (_isStreamOpen)
                {
                    _isStreamOpen = false;
                    FireOnStreamEnd();
                }
            }
            else
            {
                var parent = _current.Parent;

                if (parent == null)
                    FireOnStreamElement(_current);

                _current = parent;
            }

            _namespaces?.PopScope();
        };

        _xmlParser.OnText += value => _current?.AddChild(new Text(value));
        _xmlParser.OnComment += value => _current?.AddChild(new Comment(value));
        _xmlParser.OnCdata += value => _current?.AddChild(new Cdata(value));
    }

    protected override void Disposing()
    {
        _namespaces?.Reset();
        _namespaces = null;

        _xmlParser?.Dispose();
        _xmlParser = null;
    }

    public void Reset()
    {
        ThrowIfDisposed();
        _current = null;
        _namespaces.Reset();
        _isStreamOpen = false;
    }

    public bool TryParse(byte[] buffer, int length, out ExpatParserError error, bool isFinalBlock = false)
        => _xmlParser.TryParse(buffer, length, out error, isFinalBlock);

    public void Parse(byte[] buffer, int length, bool isFinalBlock = false)
    {
        ThrowIfDisposed();
        _xmlParser.Parse(buffer, length, isFinalBlock);
    }
}
