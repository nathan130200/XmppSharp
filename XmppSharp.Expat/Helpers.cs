#pragma warning disable

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using XmppSharp.Collections;
using static XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

public static class Helpers
{
    public static string GetMessage(this Error code)
        => Marshal.PtrToStringAnsi(XML_ErrorString(code));
}
