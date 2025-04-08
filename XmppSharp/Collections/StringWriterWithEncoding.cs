using System.Text;

namespace XmppSharp.Dom;

public sealed class StringWriterWithEncoding : StringWriter
{
    private readonly Encoding _encoding;

    public StringWriterWithEncoding(StringBuilder @out, Encoding encoding) : base(@out)
    {
        Throw.IfNull(encoding);

        _encoding = encoding;
    }

    public override Encoding Encoding
        => _encoding;
}