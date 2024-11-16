using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using XmppSharp.Collections;
using static XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

public class ExpatParser : IDisposable, IXmlLineInfo
{
    internal IntPtr _parser;
    internal volatile bool _disposed;
    internal ExpatEncoding _encoding;
    internal GCHandle _userData;
    internal volatile bool _isCdataSection = false;
    internal StringBuilder _cdataSection = new();

    private XML_StartElementHandler _onStartElementHandler;
    private XML_EndElementHandler _onEndElementHandler;
    private XML_CdataSectionHandler _onCdataStartHandler;
    private XML_CdataSectionHandler _onCdataEndHandler;
    private XML_CharacterDataHandler _onCharacterDataHandler;
    private XML_CommentHandler _onCommentHandler;

    public ExpatParser(ExpatEncoding? encoding = default)
    {
        _encoding = encoding ?? ExpatEncoding.UTF8;
        _userData = GCHandle.Alloc(this);
        _parser = XML_ParserCreate(_encoding.Name);

        _onStartElementHandler = new(StartElementHandler);
        _onEndElementHandler = new(EndElementHandler);
        _onCdataStartHandler = new(CdataStartHandler);
        _onCdataEndHandler = new(CdataEndHandler);
        _onCharacterDataHandler = new(OnCharacterDataHandler);
        _onCommentHandler = new(CommentHandler);

        GC.KeepAlive(_onStartElementHandler);
        GC.KeepAlive(_onEndElementHandler);
        GC.KeepAlive(_onCdataStartHandler);
        GC.KeepAlive(_onCdataEndHandler);
        GC.KeepAlive(_onCharacterDataHandler);
        GC.KeepAlive(_onCommentHandler);

        Setup();
    }

    public void Reset()
    {
        ThrowIfDisposed();

        if (!XML_ParserReset(_parser, _encoding.Name))
            throw new ExpatException(this);

        var error = XML_GetErrorCode(_parser);

        if (error != Error.NONE)
            throw new ExpatException(this);

        Setup();
    }

    public void SetBillionLaughsAttackProtection(float? maxAplificationFactor, long? activationThresholdBytes)
    {
        ThrowIfDisposed();

        if (maxAplificationFactor != null)
            XML_SetBillionLaughsAttackProtectionMaximumAmplification(_parser, maxAplificationFactor.Value);

        if (activationThresholdBytes != null)
            XML_SetBillionLaughsAttackProtectionActivationThreshold(_parser, activationThresholdBytes.Value);
    }

    void Setup()
    {
        XML_SetElementHandler(_parser, _onStartElementHandler, _onEndElementHandler);
        XML_SetCdataSectionHandler(_parser, _onCdataStartHandler, _onCdataEndHandler);
        XML_SetCharacterDataHandler(_parser, _onCharacterDataHandler);
        XML_SetCommentHandler(_parser, _onCommentHandler);
        XML_SetUserData(_parser, (IntPtr)_userData);
    }

    public unsafe void Write(byte[] buffer, int len, bool isFinal = false, bool throwOnError = true)
    {
        ThrowIfDisposed();

        GCHandle ptr = default;

        try
        {
            ptr = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            var result = XML_Parse(_parser, ptr.AddrOfPinnedObject(), len, isFinal);

            if (result != Status.OK)
            {
                if (result == Status.ERROR)
                {
                    if (!XML_ParserReset(_parser, _encoding.Name) && throwOnError) // ensure we can still use same parse instance later
                    {
                        throw new AggregateException(
                            new ExpatException(this),
                            new ExpatException("The parser returned an error and the attempt to reset its internal state also failed.", Error.ABORTED)
                        );
                    }
                }

                if (throwOnError)
                    throw new ExpatException(this);
            }
        }
        finally
        {
            if (ptr.IsAllocated)
                ptr.Free();
        }
    }

    public event Action<XmppName, IReadOnlyDictionary<XmppName, string>> OnStartTag;
    public event Action<string> OnEndTag;
    public event Action<string?>? OnComment;
    public event Action<string?>? OnCdata;
    public event Action<string?>? OnText;

    static void StartElementHandler(IntPtr userData, IntPtr namePtr, IntPtr attrListPtr)
    {
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;

        // name is C-style string.
        string name = Marshal.PtrToStringAnsi(namePtr);

        var attributes = new Dictionary<XmppName, string>();

        // Get num of attributes.
        var numAttributes = XML_GetSpecifiedAttributeCount(parser._parser);

        // attributes is XML_Char** where each XML_Char* is followed by: name, value, name, value, ...
        for (int i = 0; i < numAttributes; i += 2)
        {
            // attrName = ofs * pointerSize
            var attrNamePtr = Marshal.ReadIntPtr(attrListPtr, i * IntPtr.Size);

            // attrValue = (ofs + 1) * pointerSize
            var attrValuePtr = Marshal.ReadIntPtr(attrListPtr, (i + 1) * IntPtr.Size);

            // read null-terminated string for both name and value
            attributes[Marshal.PtrToStringAnsi(attrNamePtr)] = Marshal.PtrToStringAnsi(attrValuePtr);
        }

        // invoke event
        parser.OnStartTag?.Invoke(name, attributes);
    }

    static void EndElementHandler(IntPtr userData, IntPtr namePtr)
    {
        // name is C-style string.
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;
        parser.OnEndTag?.Invoke(Marshal.PtrToStringAnsi(namePtr));
    }

    static void CdataStartHandler(IntPtr userData)
    {
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;
        parser._isCdataSection = true;
    }

    static void CdataEndHandler(IntPtr userData)
    {
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;

        if (parser._cdataSection.Length > 0)
        {
            var str = parser._cdataSection.ToString();
            parser._cdataSection.Clear();
            parser.OnCdata?.Invoke(str);
        }

        parser._isCdataSection = false;
    }

    static unsafe void OnCharacterDataHandler(IntPtr userDara, IntPtr data, int size)
    {
        // data is not C-style string
        var parser = (ExpatParser)GCHandle.FromIntPtr(userDara).Target;

        var content = parser._encoding.Encoding.GetString((byte*)data, size);

        if (!parser._isCdataSection)
            parser.OnText?.Invoke(content);
        else
            parser._cdataSection.Append(content);
    }

    static void CommentHandler(IntPtr userData, IntPtr valuePtr)
    {
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;
        var value = Marshal.PtrToStringAnsi(valuePtr);
        parser.OnComment?.Invoke(value);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_parser != IntPtr.Zero)
        {
            XML_ParserFree(_parser);
            _parser = IntPtr.Zero;
        }

        if (_userData.IsAllocated)
            _userData.Free();

        _userData = default;

        _onStartElementHandler = null;
        _onEndElementHandler = null;
        _onCdataStartHandler = null;
        _onCdataEndHandler = null;
        _onCharacterDataHandler = null;
        _onCommentHandler = null;

        _cdataSection?.Clear();
        _cdataSection = null;

        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    public bool HasLineInfo() => !_disposed;

    public int LineNumber
    {
        get
        {
            if (_disposed)
                return -1;

            return XML_GetCurrentLineNumber(_parser);
        }
    }

    public int LinePosition
    {
        get
        {
            if (_disposed)
                return -1;

            return XML_GetCurrentColumnNumber(_parser);
        }
    }
}
