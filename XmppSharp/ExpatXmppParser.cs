using Expat;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp;

public interface IXmppParser : IDisposable
{
    event Action<StreamStream>? OnStreamStart;
    event Action<XmppElement>? OnStreamElement;
    event Action? OnStreamEnd;
    void Reset();
}

public sealed class ExpatXmppParser : IXmppParser
{
    readonly object _syncRoot = new();

    internal XmppElement? _current;
    internal ExpatParser? _xmlParser;
    internal XmppNamespaceStack? _namespaces;
    internal volatile bool _disposed;
    internal volatile bool _started;

    public ExpatParser? XmlParser => _xmlParser;

    public event Action<StreamStream>? OnStreamStart;
    public event Action<XmppElement>? OnStreamElement;
    public event Action? OnStreamEnd;

    public ExpatXmppParser(ExpatEncoding encoding, bool strict = true)
    {
        _namespaces = new();

        _xmlParser = new ExpatParser(encoding, strict);
        _xmlParser.OnStartElement += HandleStartElement;
        _xmlParser.OnEndElement += HandleEndElement;
        _xmlParser.OnText += HandleText;
        _xmlParser.OnComment += HandleComment;
        _xmlParser.OnCdata += HandleCdata;
    }

    void HandleStartElement(string name, IReadOnlyDictionary<string, string> attrs)
    {
        _namespaces!.PushScope();

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
            if (!_started)
            {
                _started = true;
                OnStreamStart?.Invoke(start);
            }
        }
        else
        {
            _current?.AddChild(element);
            _current = element;
        }
    }

    void HandleEndElement(string name)
    {
        _namespaces!.PopScope();

        if (name == "stream:stream")
        {
            if (_started)
            {
                _started = false;
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
    }

    void HandleText(string value)
    {
        _current?.AddChild(new XmppText(value));
    }

    void HandleComment(string value)
    {
        _current?.AddChild(new XmppComment(value));
    }

    void HandleCdata(string value)
    {
        _current?.AddChild(new XmppCdata(value));
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

        GC.SuppressFinalize(this);

        lock (_syncRoot)
        {
            _namespaces?.Clear();
            _namespaces = null;

            _xmlParser?.Dispose();
            _xmlParser = null;
        }
    }

    public void Reset()
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            _current = null;
            _started = false;
            _namespaces!.Reset();
            _xmlParser!.Reset();
        }
    }

    public bool TryParse(byte[] buffer, int length, out ExpatParserError error, bool isFinalBlock = false)
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            return _xmlParser!.TryParse(buffer, length, out error, isFinalBlock);
        }
    }

    public void Parse(byte[] buffer, int length, bool isFinalBlock = false)
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            _xmlParser!.Parse(buffer, length, isFinalBlock);
        }
    }
}