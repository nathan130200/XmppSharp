#pragma warning disable

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using XmppSharp.Collections;
using static XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

public class ExpatParser : IDisposable
{
    internal IntPtr _handle;
    internal volatile bool _disposed;
    internal Encoding _encoding;
    internal GCHandle _userData;

    public ExpatParser(Encoding? encoding = default)
    {
        _encoding = encoding ?? Encoding.UTF8;

        _handle = XML_ParserCreate(_encoding.WebName.ToUpper());
        _userData = GCHandle.Alloc(this);

        Setup();
    }

    public void Reset()
    {
        ThrowIfDisposed();
        XML_ParserReset(_handle, _encoding.WebName);
        Setup();
    }

    void Setup()
    {
        XML_SetElementHandler(_handle, StartElementCallback, EndElementCallback);
        XML_SetCdataSectionHandler(_handle, OnCdataStartCallback, OnCdataEndCallback);
        XML_SetCharacterDataHandler(_handle, OnCharacterDataCallback);
        XML_SetCommentHandler(_handle, OnCommentCallback);
        XML_SetUserData(_handle, (IntPtr)_userData);
    }

    public void Write(byte[] buffer, int len, bool isFinal = false)
    {
        ThrowIfDisposed();

        GCHandle handle = default;

        try
        {
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var result = XML_Parse(_handle, handle.AddrOfPinnedObject(), len, isFinal);

            if (result != Status.OK)
                throw new ExpatException(this);
        }
        finally
        {
            if (handle.IsAllocated)
                handle.Free();
        }
    }

    volatile bool _isCdataSection = false;
    StringBuilder _cdataSection = new StringBuilder();

    public event Action<string, IReadOnlyDictionary<string, string>> OnStartTag;
    public event Action<string> OnEndTag;
    public event Action<string?>? OnComment;
    public event Action<string?>? OnCdata;
    public event Action<string?>? OnText;

    void FireOnStartTag(string tagName, Dictionary<string, string> attrs) => OnStartTag?.Invoke(tagName, attrs);
    void FireOnEndTag(string tagName) => OnEndTag?.Invoke(tagName);
    void FireOnComment(string? value) => OnComment?.Invoke(value);
    void FireOnCdata(string? value) => OnCdata?.Invoke(value);
    void FireOnText(string? value) => OnText?.Invoke(value);

    static void StartElementCallback(IntPtr userData, IntPtr namePtr, IntPtr attrListPtr)
    {
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;

        // tag name is C-style string.
        string tagName = Marshal.PtrToStringAnsi(namePtr);

        var attributes = new Dictionary<string, string>();

        // Get num of attributes.
        var numAttributes = XML_GetSpecifiedAttributeCount(parser._handle);

        // XML_Char** where each XML_Char* is followed by: name, value, name, value, ...
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
        parser.FireOnStartTag(tagName, attributes);
    }

    static void EndElementCallback(IntPtr userData, IntPtr namePtr)
    {
        // tag name is C-style string.
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;
        var tagName = Marshal.PtrToStringAnsi(namePtr);
        parser.FireOnEndTag(tagName);
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
            parser.FireOnCdata(str);
        }

        parser._isCdataSection = false;
    }

    static unsafe void OnCharacterDataCallback(IntPtr userDara, IntPtr data, int size)
    {
        // data is not C-style string
        var parser = (ExpatParser)GCHandle.FromIntPtr(userDara).Target;

        var content = parser._encoding.GetString((byte*)data, size);

        if (!parser._isCdataSection)
            parser.FireOnText(content);
        else
            parser._cdataSection.Append(content);
    }

    static void OnCommentCallback(IntPtr userData, IntPtr valuePtr)
    {
        var parser = (ExpatParser)GCHandle.FromIntPtr(userData).Target;
        var value = Marshal.PtrToStringAnsi(valuePtr);
        parser.FireOnComment(value);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_handle != IntPtr.Zero)
        {
            XML_ParserFree(_handle);
            _handle = IntPtr.Zero;
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
