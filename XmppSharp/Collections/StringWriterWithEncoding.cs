using System.Text;

namespace XmppSharp.Collections;

public sealed class StringWriterWithEncoding : StringWriter
{
    private readonly Encoding _encoding;

    public StringWriterWithEncoding(Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        _encoding = encoding;
    }

    public StringWriterWithEncoding(IFormatProvider? formatProvider, Encoding encoding) : base(formatProvider)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        _encoding = encoding;
    }

    public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        _encoding = encoding;
    }

    public StringWriterWithEncoding(StringBuilder sb, IFormatProvider? formatProvider, Encoding encoding) : base(sb, formatProvider)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        _encoding = encoding;
    }

    public override Encoding Encoding
        => _encoding;
}