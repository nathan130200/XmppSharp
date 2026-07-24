using System.Text;

namespace XmppSharp.Serialization;

public sealed class StringWriterWithEncoding : StringWriter
{
    readonly Encoding _encoding;

    public StringWriterWithEncoding() : this(Encoding.UTF8)
    {
    }

    public StringWriterWithEncoding(Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        _encoding = encoding;
    }

    public override Encoding Encoding => _encoding;
}
