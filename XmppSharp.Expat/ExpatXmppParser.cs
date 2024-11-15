using System.Text;
using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public class ExpatXmppParser : XmppParser
{
    private ExpatParser _parser;
    private Element? _current;

    public ExpatXmppParser(Encoding? encoding = default)
    {
        _parser = new ExpatParser(encoding);

        _parser.OnStartTag += (name, attrs) =>
        {
            var attrName = name.HasPrefix ? $"xmlns:{name.Prefix}" : "xmlns";
            var element = ElementFactory.CreateElement(name, attrs.GetValueOrDefault(attrName));

            foreach (var (key, value) in attrs)
                element.SetAttribute(key, value);

            if (element is StreamStream stream)
                FireOnStreamStart(stream);
            else
            {
                _current?.AddChild(element);
                _current = element;
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
        _parser.Reset();
    }

    public void Write(byte[] buffer, int length, bool isFinal = false)
    {
        ThrowIfDisposed();
        _parser.Write(buffer, length);
    }
}
