#pragma warning disable

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using XmppSharp.Collections;
using static XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

public class ExpatEncoding
{
    public static ExpatEncoding ISO88591 = new("ISO-8859-1", Encoding.Latin1);
    public static ExpatEncoding ASCII = new("US-ASCII", Encoding.ASCII);
    public static ExpatEncoding UTF8 = new("UTF-8", Encoding.UTF8);
    public static ExpatEncoding UTF16 = new("UTF-16", Encoding.Unicode);
    public static ExpatEncoding UTF16BE = new("UTF-16BE", Encoding.BigEndianUnicode);
    public static ExpatEncoding UTF16LE = new("UTF-16LE", Encoding.Unicode);

    public string Name { get; }
    public Encoding Encoding { get; }

    ExpatEncoding(string name, Encoding enc)
    {
        Name = name;
        Encoding = enc;
    }
}
