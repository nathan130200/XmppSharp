using XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

using static PInvoke;

public class ExpatException : Exception
{
    public long LineNumber { get; internal set; }
    public long LinePosition { get; internal set; }
    public int ByteIndex { get; internal set; }
    public int ByteCount { get; internal set; }
    public Error Code { get; init; }

    public ExpatException()
    {

    }

    internal unsafe void SetParserInfo(nint parser)
    {
        if (parser == IntPtr.Zero)
            return;

        LineNumber = GetCurrentLineNumber(parser);
        LinePosition = GetCurrentColumnNumber(parser);
        ByteIndex = GetCurrentByteIndex(parser);
        ByteCount = GetCurrentByteCount(parser);
    }

    public ExpatException(string? message) : base(message)
    {

    }

    public ExpatException(string? message, Exception? innerException) : base(message, innerException)
    {

    }

    public ExpatException(Error code) : base(GetErrorString(code))
    {
        Code = code;
    }

    public ExpatException(Error code, Exception? innerException) : base(GetErrorString(code), innerException)
    {
        Code = code;
    }
}
