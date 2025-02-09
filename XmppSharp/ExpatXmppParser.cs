using Expat;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public sealed class ExpatXmppParser : IDisposable
{
    internal XmppElement? _current;
    internal ExpatParser? _xmlParser;
    internal XmppNamespaceStack? _namespaces;
    internal volatile bool _streamOpen;
    internal volatile bool _disposed;

    public ExpatParser? UnderlyingParser => _xmlParser;

    public event Action<StreamStream>? OnStreamStart;
    public event Action<XmppElement>? OnStreamElement;
    public event Action? OnStreamEnd;

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

            var element = XmppElementFactory.Create(tagName, _namespaces.LookupNamespace(tagName.Prefix), _current);

            foreach (var (key, value) in attrs)
                element.SetAttribute(key, value);

            if (element is StreamStream start)
            {
                if (!_streamOpen)
                {
                    _streamOpen = true;
                    OnStreamStart?.Invoke(start);
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
            _namespaces.PopScope();

            if (name == "stream:stream")
            {
                if (_streamOpen)
                {
                    _streamOpen = false;
                    OnStreamEnd?.Invoke();
                }
            }
            else
            {
                var parent = _current?.Parent;

                if (parent == null && _current != null)
                    OnStreamElement?.Invoke(_current);

                _current = parent;
            }
        };

        _xmlParser.OnText += value
            => _current?.AddChild(new XmppText(value));

        _xmlParser.OnComment += value
            => _current?.AddChild(new XmppComment(value));

        _xmlParser.OnCdata += value
            => _current?.AddChild(new XmppCdata(value));
    }

    void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _namespaces?.Reset();
        _namespaces = null;

        if (_xmlParser != null)
        {
            _xmlParser.Dispose();
            _xmlParser = null;
        }
    }

    public void Reset()
    {
        ThrowIfDisposed();
        _current = null;
        _namespaces!.Reset();
        _streamOpen = false;
    }

    public bool TryParse(byte[] buffer, int length, out ExpatParserError error, bool isFinalBlock = false)
    {
        ThrowIfDisposed();
        return _xmlParser!.TryParse(buffer, length, out error, isFinalBlock); ;
    }

    public void Parse(byte[] buffer, int length, bool isFinalBlock = false)
    {
        ThrowIfDisposed();
        _xmlParser!.Parse(buffer, length, isFinalBlock);
    }
}