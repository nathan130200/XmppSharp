using XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

using static PInvoke;

public class ExpatException : Exception
{
	private volatile bool _haveParserInfo;
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
		if (parser == nint.Zero)
			return;

		if (!_haveParserInfo)
		{
			_haveParserInfo = true;
			LineNumber = GetCurrentLineNumber(parser);
			LinePosition = GetCurrentColumnNumber(parser);
			ByteIndex = GetCurrentByteIndex(parser);
			ByteCount = GetCurrentByteCount(parser);
		}
	}

	public ExpatException(string? message) : base(message)
	{

	}

	public ExpatException(string? message, Exception? innerException) : base(message, innerException)
	{

	}

	public ExpatException(Error error) : base(GetMessage(error))
	{
		Code = error;
	}

	public ExpatException(Error error, Exception? innerException) : base(GetMessage(error), innerException)
	{
		Code = error;
	}

	static string GetMessage(Error code)
	{
		if (!Enum.IsDefined(code))
			return "Unknown error";

		if (code == Error.None)
			return "No errors.";

		return GetErrorString(code);
	}
}
