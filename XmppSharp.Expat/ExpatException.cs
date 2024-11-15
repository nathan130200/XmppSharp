#pragma warning disable

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using XmppSharp.Collections;
using static XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

public class ExpatException : Exception
{
    private readonly string _errorMessage;

    public Error Code { get; }

    public int Line { get; }
    public int Column { get; }

    public override string Message => _errorMessage;

    public ExpatException(ExpatParser parser) : base()
    {
        if (!parser._disposed)
        {
            _errorMessage = XML_GetErrorCode(parser._handle).GetMessage();
            Line = XML_GetCurrentLineNumber(parser._handle);
            Column = XML_GetCurrentColumnNumber(parser._handle);
        }
    }

    public ExpatException(ExpatParser parser, Error code) : base()
    {
        _errorMessage = code.GetMessage();

        if (!parser._disposed)
        {
            Line = XML_GetCurrentLineNumber(parser._handle);
            Column = XML_GetCurrentColumnNumber(parser._handle);
        }
    }

    public ExpatException(Error code) : base()
    {
        _errorMessage = code.GetMessage();
        Code = code;
    }
}