using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Protocol.Base;
using XmppSharp.Utilities;

namespace XmppSharp.Parser;

public class ExpatXmppParser : XmppParser
{
    private ExpatParser _parser;
    private Element? _current;
    private NamespaceStack _namespaces;

#if NET9_0_OR_GREATER
    private readonly Lock _syncRoot = new();
#else
    private readonly object _syncRoot = new();
#endif

    public ExpatXmppParser(ExpatEncoding encoding)
    {
        _namespaces = new NamespaceStack();
        _parser = new ExpatParser(encoding);

        _parser.OnStartTag += (name, attrs) =>
        {
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

            if (element is StreamStream start)
                FireOnStreamStart(start);
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
            if (name == "stream:stream")
                FireOnStreamEnd();
            else
            {
                var parent = _current.Parent;

                if (parent == null)
                    FireOnStreamElement(_current);

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
    }

    public void Write(byte[] buffer, int length, bool isFinal = false, bool throwOnError = true)
    {
        ThrowIfDisposed();
        _parser.Write(buffer, length, isFinal, throwOnError);
    }
}
