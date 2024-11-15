#pragma warning disable

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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

public class ExpatParser : IDisposable
{
    internal IntPtr _parser;
    internal volatile bool _disposed;
    internal ExpatEncoding _encoding;
    internal GCHandle _userData;

    public ExpatParser(ExpatEncoding? encoding = default)
    {
        _encoding = encoding ?? ExpatEncoding.UTF8;
        _userData = GCHandle.Alloc(this);
        _parser = XML_ParserCreate(_encoding.Name);

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
        XML_SetElementHandler(_parser, StartElementCallback, EndElementCallback);
        XML_SetCdataSectionHandler(_parser, OnCdataStartCallback, OnCdataEndCallback);
        XML_SetCharacterDataHandler(_parser, OnCharacterDataCallback);
        XML_SetCommentHandler(_parser, OnCommentCallback);
        XML_SetUserData(_parser, (IntPtr)_userData);
    }

    public unsafe void Write(byte[] buffer, int len, bool isFinal = false)
    {
        ThrowIfDisposed();

        GCHandle ptr = default;

        try
        {
            ptr = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            var result = XML_Parse(_parser, ptr.AddrOfPinnedObject(), len, isFinal);

            if (result != Status.OK)
                throw new ExpatException(this);
        }
        finally
        {
            if (ptr.IsAllocated)
                ptr.Free();
        }
    }

    volatile bool _isCdataSection = false;
    StringBuilder _cdataSection = new StringBuilder();

    public event Action<XmppName, IReadOnlyDictionary<XmppName, string>> OnStartTag;
    public event Action<string> OnEndTag;
    public event Action<string?>? OnComment;
    public event Action<string?>? OnCdata;
    public event Action<string?>? OnText;

    static void StartElementCallback(IntPtr userData, IntPtr namePtr, IntPtr attrListPtr)
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

    static void EndElementCallback(IntPtr userData, IntPtr namePtr)
    {
        // name is C-style string.
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;
        parser.OnEndTag?.Invoke(Marshal.PtrToStringAnsi(namePtr));
    }

    static void OnCdataStartCallback(IntPtr userData)
    {
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;
        parser._isCdataSection = true;
    }

    static void OnCdataEndCallback(IntPtr userData)
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

    static unsafe void OnCharacterDataCallback(IntPtr userDara, IntPtr data, int size)
    {
        // data is not C-style string
        var parser = (ExpatParser)GCHandle.FromIntPtr(userDara).Target;

        var content = parser._encoding.Encoding.GetString((byte*)data, size);

        if (!parser._isCdataSection)
            parser.OnText?.Invoke(content);
        else
            parser._cdataSection.Append(content);
    }

    static void OnCommentCallback(IntPtr userData, IntPtr valuePtr)
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

        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }
}
