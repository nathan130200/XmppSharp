using System.Text;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

public sealed class Text : Node
{
    string? _value;

    StringBuilder? _builder;

    public Text()
    {
        _value = string.Empty;
    }

    public Text(string? value)
    {
        _value = value ?? string.Empty;
    }

    public string Value
    {
        get
        {
            if (_value is not null)
                return _value;

            _value = _builder!.ToString();
            _builder = null;

            return _value;
        }
    }

    public void Append(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return;

        if (_builder is null)
        {
            _builder = new StringBuilder(_value);
            _value = null;
        }

        _builder.Append(value);
    }

    public void AppendUtf8(ReadOnlySpan<byte> bytes)
    {
        var numChars = Encoding.UTF8.GetCharCount(bytes);

        Span<char> span = numChars < 256
            ? stackalloc char[numChars]
            : new char[numChars];

        Encoding.UTF8.GetChars(bytes, span);

        Append(span);
    }

    public override Node Clone()
    {
        if (_builder is null)
            return new Text(_value);

        return new Text
        {
            _value = null,
            _builder = new StringBuilder(_builder.ToString())
        };
    }

    public override void WriteTo(XmlWriter writer)
    {
        writer.WriteString(Value);
    }

    public override string ToString() => Value;
}