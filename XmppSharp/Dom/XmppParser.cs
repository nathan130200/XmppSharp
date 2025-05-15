using System.Xml;
using Expat;
using Stream = XmppSharp.Protocol.Base.Stream;

namespace XmppSharp.Dom;

public interface IXmppParser : IDisposable
{
    event Action<Stream>? OnStreamStart;
    event Action<XmppElement>? OnStreamElement;
    event Action? OnStreamEnd;
    void Reset();
}

public sealed class XmppParser : IXmppParser
{
    readonly Lock _syncRoot = new();

    internal XmppElement? _current;
    internal ExpatParser? _parser;
    internal XmlNamespaceManager? _namespaces;
    internal volatile bool _disposed;
    internal volatile bool _started;

    public event Action<Stream>? OnStreamStart;
    public event Action<XmppElement>? OnStreamElement;
    public event Action? OnStreamEnd;

    public XmppParser(ExpatEncoding? encoding = default, bool strict = true)
    {
        _namespaces = new(new NameTable());

        _parser = new ExpatParser(encoding, strict);
        _parser.OnStartElement += HandleStartElement;
        _parser.OnEndElement += HandleEndElement;
        _parser.OnText += HandleText;
        _parser.OnCdata += HandleCdata;
    }

    void HandleStartElement(string tagName, IReadOnlyDictionary<string, string> attrs)
    {
        _namespaces!.PushScope();

        foreach (var (key, value) in attrs
            .Where(x => x.Key == "xmlns" || x.Key.StartsWith("xmlns:")))
        {
            var hasPrefix = Xml.ExtractQualifiedName(key, out _, out var prefix);

            if (!hasPrefix)
                _namespaces.AddNamespace(string.Empty, value);
            else
                _namespaces.AddNamespace(prefix, value);
        }

        {
            var hasPrefix = Xml.ExtractQualifiedName(tagName, out var prefix, out _);
            var element = XmppElementFactory.Create(tagName, _namespaces.LookupNamespace(hasPrefix ? prefix! : string.Empty), _current);

            foreach (var (nsPrefix, value) in _namespaces.GetNamespacesInScope(XmlNamespaceScope.Local))
            {
                if (string.IsNullOrWhiteSpace(nsPrefix))
                    element.SetNamespace(value);
                else
                    element.SetNamespace(nsPrefix, value);
            }

            foreach (var (key, value) in attrs)
                element.SetAttribute(key, value);

            if (element is Protocol.Base.Stream start)
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
            while (_namespaces?.PopScope() == true)
                ;

            _namespaces = null;

            _parser?.Dispose();
            _parser = null;
        }
    }

    public void Reset()
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            _current = null;
            _started = false;

            while (_namespaces?.PopScope() == true)
                ;

            _parser!.Reset();
        }
    }

    public bool TryParse(byte[] buffer, int length, out ExpatParserError error, bool isFinalBlock = false)
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            return _parser!.TryParse(buffer, length, out error, isFinalBlock);
        }
    }

    public void Parse(byte[] buffer, int length, bool isFinalBlock = false)
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            _parser!.Parse(buffer, length, isFinalBlock);
        }
    }

    public void Suspend(bool resumable)
    {
        ThrowIfDisposed();

        lock (_syncRoot)
            _parser!.Suspend(resumable);
    }

    public void Resume()
    {
        ThrowIfDisposed();

        lock (_syncRoot)
            _parser!.Resume();
    }
}