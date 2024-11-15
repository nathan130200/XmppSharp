using System.Collections;
using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public class NamespaceStack
{
    private readonly Stack<Hashtable> _stack = new();
    private readonly object _syncRoot = new();

    public NamespaceStack()
    {
        PushScope();
        AddNamespace("xml", Namespaces.Xml);
        AddNamespace("xmlns", Namespaces.Xmlns);
    }

    public void PushScope()
    {
        lock (_syncRoot)
            _stack.Push(new Hashtable());
    }

    public void PopScope()
    {
        lock (_syncRoot)
        {
            if (_stack.Count > 1)
                _stack.Pop();
        }
    }

    public void AddNamespace(string prefix, string uri)
    {
        lock (_syncRoot)
        {
            var dict = _stack.Peek();
            dict.Add(prefix, uri);
        }
    }

    public string? LookupNamespace(string? prefix)
    {
        prefix ??= string.Empty;

        lock (_syncRoot)
        {
            foreach (var entry in _stack)
            {
                if (entry.ContainsKey(prefix))
                    return (string)entry[prefix];
            }
        }

        return null;
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            while (_stack.Count > 1)
                _stack.Pop();
        }
    }

    public string DefaultNamespace
        => LookupNamespace(string.Empty);
}

public class ExpatXmppParser : XmppParser
{
    private ExpatParser _parser;
    private Element? _current;
    private NamespaceStack _namespaces;

    public ExpatXmppParser(ExpatEncoding encoding)
    {
        _namespaces = new NamespaceStack();
        _parser = new ExpatParser(encoding);

        _parser.OnStartTag += (name, attrs) =>
        {
            Console.WriteLine("[OnStartTag]: name: " + name + " (attributes: " + attrs.Count + ")");

            _namespaces.PushScope();

            foreach (var (key, value) in attrs)
            {
                if (key == "xmlns")
                    _namespaces.AddNamespace(string.Empty, value);
                else if (key.Prefix == "xmlns")
                    _namespaces.AddNamespace(key.LocalName, value);
            }

            var element = ElementFactory.CreateElement(name, _namespaces.LookupNamespace(name.Prefix));

            foreach (var (key, value) in attrs)
                element.SetAttribute(key, value);

            if (element is StreamStream stream)
                AsyncHelper.RunAsync(FireOnStreamStart, stream);
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

        _parser.OnEndTag += name =>
        {
            Console.WriteLine("[OnEndTag] name: " + name);

            if (name == "stream:stream")
                AsyncHelper.RunAsync(FireOnStreamEnd);
            else
            {
                var parent = _current.Parent;

                if (parent == null)
                    AsyncHelper.RunAsync(FireOnStreamElement, _current);

                _current = parent;
            }

            _namespaces.PopScope();
        };

        _parser.OnText += value => _current?.AddChild(new Text(value));
        _parser.OnComment += value => _current?.AddChild(new Comment(value));
        _parser.OnCdata += value => _current?.AddChild(new Cdata(value));
    }

    protected override void DisposeCore()
    {
        _parser?.Dispose();
        _parser = null;
    }

    public void Reset()
    {
        ThrowIfDisposed();
        _current = null;
        _namespaces.Clear();
        _parser.Reset();
    }

    public void Write(byte[] buffer, int length, bool isFinal = false)
    {
        ThrowIfDisposed();
        _parser.Write(buffer, length, isFinal);
    }
}
