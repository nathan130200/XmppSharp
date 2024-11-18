using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Protocol.Base;
using XmppSharp.Utilities;

namespace XmppSharp.Parser;

public class ExpatXmppParser : XmppParser, IXmppStreamTokenizer
{
    private ExpatParser _xmlParser;
    private Element? _current;
    private NamespaceStack? _namespaces;
    private volatile bool _isStreamOpen;

    public ExpatParser XmlParser => _xmlParser;

    public ExpatXmppParser(ExpatEncoding encoding)
    {
        _namespaces = new NamespaceStack();
        _xmlParser = new ExpatParser(encoding);

        _xmlParser.OnStartTag += (name, attrs) =>
        {
            _namespaces.PushScope();

            foreach (var (key, value) in attrs)
            {
                if (key == "xmlns")
                    _namespaces.AddNamespace(string.Empty, value);
                else if (key.Prefix == "xmlns")
                    _namespaces.AddNamespace(key.LocalName, value);
            }

            var element = ElementFactory.CreateElement(name, _namespaces.LookupNamespace(name.Prefix), _current);

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
                if (_current == null)
                    _current = element;
                else
                {
                    _current.AddChild(element);
                    _current = element;
                }
            }
        };

        _xmlParser.OnEndTag += name =>
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

    protected override void DisposeCore()
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

    public void Write(byte[] buffer, int length, bool isFinal = false, bool throwOnError = true)
    {
        ThrowIfDisposed();
        _xmlParser.Write(buffer, length, isFinal, throwOnError);
    }

    void IXmppStreamTokenizer.Write(byte[] buffer, int length)
        => Write(buffer, length, false);
}
