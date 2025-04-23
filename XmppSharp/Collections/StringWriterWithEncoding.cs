using System.Text;

namespace XmppSharp.Collections;

public sealed class StringWriterWithEncoding : StringWriter
{
    private readonly Encoding _encoding;

    public StringWriterWithEncoding(StringBuilder @out, Encoding encoding) : base(@out)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        _encoding = encoding;
    }

    public override Encoding Encoding
        => _encoding;
}