namespace XmppSharp.Expat;

using System.Runtime.InteropServices;
using System.Text;
using XmppSharp.Expat.Native;
using static XmppSharp.Expat.Native.PInvoke;

public unsafe class ExpatParser : IDisposable
{
	public event Action<string, string?, bool?>? OnProlog;
	public event Action<string, string?>? OnProcessingInstruction;
	public event Action<string, IReadOnlyDictionary<string, string>>? OnElementStart;
	public event Action<string>? OnElementEnd;
	public event Action<string>? OnCdata;
	public event Action<string>? OnComment;
	public event Action<string>? OnText;

	//private StartElementHandler _startElementHandler;
	//private EndElementHandler _endElementHandler;
	//private CommentHandler _commentHandler;
	//private CdataSectionHandler _cdataStartHandler, _cdataEndHandler;
	//private ProcessingInstructionHandler _processingInstructionHandler;
	//private PrologHandler _prologHandler;

	private volatile bool _disposed;

	private GCHandle _class;
	private nint _cpointer;
	private StringBuilder _cdata;
	private bool _isCdata;

	private string _encodingName;
	private Encoding _encoding;

	public ExpatParser(EncodingType type = EncodingType.UTF8)
	{
		if (!Enum.IsDefined(type))
			type = EncodingType.UTF8;

		(_encodingName, _encoding) = type.GetEncoding();

		_cpointer = ParserCreate(_encodingName);

		if (_cpointer == nint.Zero)
		{
			_disposed = true;
			throw new ExpatException("Unable to create parser instance.");
		}

		_cdata = new StringBuilder();
		_class = GCHandle.Alloc(this, GCHandleType.Weak);

		Init();
	}

	public void SetEncoding(EncodingType type)
	{
		ThrowIfDisposed();
		(_encodingName, _encoding) = type.GetEncoding();
		var result = PInvoke.SetEncoding(_cpointer, _encodingName);
		ThrowIfFailed(result, _cpointer);
	}

	public nint CPointer
		=> _cpointer;

	public int ByteIndex
	{
		get
		{
			if (_disposed)
				return -1;

			return GetCurrentByteIndex(_cpointer);
		}
	}

	public int ByteCount
	{
		get
		{
			if (_disposed)
				return -1;

			return GetCurrentByteCount(_cpointer);
		}
	}

	public void SetExceptionInfo(ExpatException ex)
	{
		if (_disposed)
			return;

		ex.SetParserInfo(_cpointer);
	}

	public long LineNumber
	{
		get
		{
			if (_disposed)
				return -1;

			return GetCurrentLineNumber(_cpointer);
		}
	}

	public long ColumnNumber
	{
		get
		{
			if (_disposed)
				return -1;

			return GetCurrentColumnNumber(_cpointer);
		}
	}

	protected void FireOnElementStart(string name, Dictionary<string, string> attrs)
		=> OnElementStart?.Invoke(name, attrs.AsReadOnly());

	protected void FireOnElementEnd(string name)
		=> OnElementEnd?.Invoke(name);

	protected void FireOnComment(string value)
		=> OnComment?.Invoke(value);

	protected void FireOnCdata(string value)
		=> OnCdata?.Invoke(value);

	protected void FireOnText(string value)
		=> OnText?.Invoke(value);

	protected void FireOnProlog(string version, string? encoding, bool? standalone)
		=> OnProlog?.Invoke(version, encoding, standalone);

	protected void FireOnProcessingInstruction(string target, string? data)
		=> OnProcessingInstruction?.Invoke(target, data);

	public static ExpatParser GetParser(nint ptr)
		=> (ExpatParser)GCHandle.FromIntPtr(ptr).Target;

	static void StartElementHandlerImpl(nint cls, XML_Char* _name, XML_Char** _attrs)
	{
		var parser = GetParser(cls);
		var name = new string(_name);
		var attrs = new Dictionary<string, string>();

		var count = GetSpecifiedAttributeCount(parser._cpointer);

		for (var i = 0; i < count; i += 2)
		{
			var key = new string(_attrs[i]);
			var value = new string(_attrs[i + 1]);
			attrs[key] = value;
		}

		parser.FireOnElementStart(name, attrs);
	}

	static void EndElementHandlerImpl(nint cls, XML_Char* name)
	{
		var parser = GetParser(cls);
		parser.FireOnElementEnd(new string(name));
	}

	static void CdataStartHandlerImpl(nint cls)
	{
		var parser = GetParser(cls);
		parser._isCdata = true;
	}

	static void CdataEndHandlerImpl(nint cls)
	{
		var parser = GetParser(cls);
		parser._isCdata = false;

		var text = parser._cdata.ToString();
		parser._cdata.Clear();
		parser.FireOnCdata(text);
	}

	static void CharacterDataHandlerImpl(nint cls, XML_Char* data, int len)
	{
		var parser = GetParser(cls);

		var buf = new string(data, 0, len);

		if (parser._isCdata)
			parser._cdata.Append(buf);
		else
			parser.FireOnText(buf);
	}

	static void CommentHandlerImpl(nint cls, XML_Char* str)
	{
		var parser = GetParser(cls);
		parser.FireOnComment(new string(str));
	}

	static void PrologHandlerImpl(nint cls, XML_Char* version, XML_Char* encoding, int standalone)
	{
		var parser = GetParser(cls);

		parser.FireOnProlog(
			new string(version),
			encoding != null ? new string(encoding) : string.Empty,
			standalone == -1 ? null : standalone == 1);
	}

	static void ProcessingInstructionHandlerImpl(nint cls, XML_Char* target, XML_Char* data)
	{
		var parser = GetParser(cls);
		parser.FireOnProcessingInstruction(new string(target), new string(data));
	}

	protected void ThrowIfDisposed()
		=> ObjectDisposedException.ThrowIf(_disposed, this);

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		_cdata?.Clear();
		_cdata = null;

		if (_class.IsAllocated)
		{
			_class.Free();
			_class = default;
		}

		if (_cpointer != 0)
		{
			ParserFree(_cpointer);
			_cpointer = 0;
		}

		GC.SuppressFinalize(this);
	}

	public void Reset()
	{
		ThrowIfDisposed();

		var status = ParserReset(_cpointer, _encodingName);

		if (!status)
			ThrowIfFailed(Status.Error, _cpointer);

		Init();
	}

	static void ThrowIfFailed(Status s, nint ptr)
	{
		if (s == Status.Error)
		{
			var ex = new ExpatException(GetErrorCode(ptr));
			ex.SetParserInfo(ptr);
			throw ex;
		}
	}

	public void Stop(bool resumable)
	{
		ThrowIfDisposed();

		var status = StopParser(_cpointer, resumable);
		ThrowIfFailed(status, _cpointer);
	}

	public void Resume()
	{
		var status = ResumeParser(_cpointer);
		ThrowIfFailed(status, _cpointer);
	}

	public void Write(byte[] buffer, int count, bool isFinal = false)
	{
		ThrowIfDisposed();

		var temp = new byte[count];
		Buffer.BlockCopy(buffer, 0, temp, 0, count);
		var status = Parse(_cpointer, temp, count, isFinal);
		ThrowIfFailed(status, _cpointer);
	}

	public void WriteInplace(byte[] buffer, int count, bool isFinal = false)
	{
		ThrowIfDisposed();

		var status = Parse(_cpointer, buffer, count, isFinal);
		ThrowIfFailed(status, _cpointer);
	}

	internal void Init()
	{
		ThrowIfDisposed();

		SetUserData(_cpointer, (nint)_class);
		SetElementHandler(_cpointer, StartElementHandlerImpl, EndElementHandlerImpl);
		SetCommentHandler(_cpointer, CommentHandlerImpl);
		SetCdataSectionHandler(_cpointer, CdataStartHandlerImpl, CdataEndHandlerImpl);
		SetCharacterDataHandler(_cpointer, CharacterDataHandlerImpl);
		SetPrologHandler(_cpointer, PrologHandlerImpl);
		SetProcessingInstructionHandler(_cpointer, ProcessingInstructionHandlerImpl);
	}
}
