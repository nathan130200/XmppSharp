using System.Runtime.CompilerServices;
using System.Xml;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Expat.Native;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public partial class ExpatXmppParser : IDisposable
{
    private ExpatParser _parser;
    private Element _currentElem;
    private NameTable _nameTable = new();
    private XmlNamespaceManager _nsStack;

    public event Action<StreamStream> OnStreamStart;
    public event Action<Element> OnStreamElement;
    public event Action OnStreamEnd;

    public ExpatXmppParser(EncodingType encoding = EncodingType.UTF8)
    {
        _nsStack = new XmlNamespaceManager(_nameTable);

        _parser = new ExpatParser(encoding);

        _parser.OnElementStart += (name, attributes) =>
        {
            _nsStack.PushScope();

            var qname = new XmppName(name);

            foreach (var (key, value) in attributes)
            {
                if (key == "xmlns" || key.StartsWith("xmlns:"))
                {
                    var attrName = new XmppName(key);

                    if (attrName.HasPrefix)
                        _nsStack.AddNamespace(attrName.LocalName, value);
                    else
                        _nsStack.AddNamespace(string.Empty, value);
                }
            }

            var ns = _nsStack.LookupNamespace(qname.HasPrefix ? qname.Prefix : string.Empty);

            if (name is "iq" or "message" or "presence") // FIXME: Small work-around to ensure stanzas will be always deserialized.
                ns ??= Namespaces.Client;

            var element = ElementFactory.CreateElement(name, ns);

            foreach (var (key, value) in attributes)
                element.SetAttribute(key, value);

            if (name == "stream:stream")
                OnStreamStart?.Invoke(element as StreamStream);
            else
            {
                _currentElem?.AddChild(element);
                _currentElem = element;
            }
        };

        _parser.OnElementEnd += (name) =>
        {
            _nsStack.PopScope();

            if (name == "stream:stream")
                OnStreamEnd?.Invoke();
            else
            {
                var parent = _currentElem.Parent;

                if (parent == null)
                    OnStreamElement?.Invoke(_currentElem);
                else
                {
                    if (name != _currentElem.TagName)
                        throw new InvalidOperationException("XML end tag mismatch.");
                }

                _currentElem = parent;
            }
        };

        _parser.OnText += (text) =>
        {
            if (_currentElem == null)
                return;

            if (_currentElem.LastNode is Text node)
                node.Value += text;
            else
                _currentElem.AddChild(new Text(text));
        };

        _parser.OnCdata += value =>
        {
            _currentElem?.AddChild(new Cdata(value));
        };

        _parser.OnComment += value =>
        {
            _currentElem?.AddChild(new Comment(value));
        };
    }

    public void Reset()
    {
        ThrowIfDisposed();

        _nsStack = new(_nameTable);
        _parser.Reset();
    }

    public void Write(byte[] buffer, int count, bool isFinal = false)
    {
        ThrowIfDisposed();
        _parser.Write(buffer, count, isFinal);
    }

    public void WriteInplace(byte[] buffer, int count, bool isFinal = false)
    {
        ThrowIfDisposed();
        _parser.WriteInplace(buffer, count, isFinal);
    }

    protected volatile bool _disposed;

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _nsStack = null;

        while (_nsStack.PopScope())
            ;

        _nameTable = null;
        _parser?.Dispose();
        _parser = null;

        GC.SuppressFinalize(this);
    }
}
