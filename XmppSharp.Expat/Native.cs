using System.Runtime.InteropServices;

namespace XmppSharp.Expat;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void XML_StartElementHandler(IntPtr userData, IntPtr name, IntPtr atts);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void XML_EndElementHandler(IntPtr userData, IntPtr name);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void XML_CharacterDataHandler(IntPtr userData, IntPtr s, int len);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void XML_CommentHandler(IntPtr userData, IntPtr data);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void XML_CdataSectionHandler(IntPtr userData);

public static class Native
{
    const string LibraryName = "libexpat";

    public static string GetMessage(this Error code)
        => Marshal.PtrToStringAnsi(XML_ErrorString(code));

    [DllImport(LibraryName)]
    public static extern nint XML_ParserCreate(string encoding);

    [DllImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool XML_ParserReset(IntPtr parser, string encoding);

    [DllImport(LibraryName)]
    public static extern void XML_ParserFree(IntPtr parser);

    [DllImport(LibraryName)]
    public static extern Status XML_SetEncoding(IntPtr parser, string encoding);

    [DllImport(LibraryName)]
    public static extern Error XML_GetErrorCode(IntPtr parser);

    [DllImport(LibraryName)]
    public static extern void XML_SetElementHandler(IntPtr parser, XML_StartElementHandler start, XML_EndElementHandler end);

    [DllImport(LibraryName)]
    public static extern void XML_SetCharacterDataHandler(IntPtr parser, XML_CharacterDataHandler handler);

    [DllImport(LibraryName)]
    public static extern void XML_SetCommentHandler(IntPtr parser, XML_CommentHandler handler);

    [DllImport(LibraryName)]
    public static extern void XML_SetCdataSectionHandler(IntPtr parser, XML_CdataSectionHandler start, XML_CdataSectionHandler end);

    [DllImport(LibraryName)]
    public static extern int XML_GetSpecifiedAttributeCount(IntPtr parser);

    [DllImport(LibraryName)]
    public static extern Status XML_Parse(IntPtr parser, IntPtr s, int len, [MarshalAs(UnmanagedType.I1)] bool isFinal);

    [DllImport(LibraryName)]
    public static extern int XML_GetCurrentLineNumber(IntPtr parser);

    [DllImport(LibraryName)]
    public static extern int XML_GetCurrentColumnNumber(IntPtr parser);

    [DllImport(LibraryName)]
    public static extern IntPtr XML_ErrorString(Error code);

    [DllImport(LibraryName)]
    public static extern bool XML_SetBillionLaughsAttackProtectionMaximumAmplification(IntPtr parser, float maximumAmplificationFactor);

    [DllImport(LibraryName)]
    public static extern bool XML_SetBillionLaughsAttackProtectionActivationThreshold(IntPtr parser, long activationThresholdBytes);

    [DllImport(LibraryName)]
    public static extern void XML_SetUserData(IntPtr parser, IntPtr userData);
}
