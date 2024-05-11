using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XmppSharp.Expat;

public delegate void ExpatDeclarationHandler(string version, string? encoding, bool? standalone);
public delegate void ExpatStartElementHandler(string name, IReadOnlyDictionary<string, string> attributes);
public delegate void ExpatContentElementHandler(ContentNodeType type, string data);
public delegate void ExpatEndElementHandler(string name);

public class ExpatParser : IDisposable
{
	protected nint m_CPointer;
	private volatile bool m_bDisposed;
	private readonly string m_EncodingName;

	public event ExpatDeclarationHandler OnProlog;
	public event ExpatStartElementHandler OnElementStart;
	public event ExpatContentElementHandler OnText;
	public event ExpatEndElementHandler OnElementEnd;

	public ExpatParser(EncodingType encoding)
	{
		m_EncodingName = encoding switch
		{
			EncodingType.ASCII => "US-ASCII",
			EncodingType.ISO88591 => "",
			EncodingType.UTF16 => "UTF-16",
			EncodingType.UTF8 or _ => "UTF-8"
		};

		m_CPointer = PInvoke.XML_ParserCreate(m_EncodingName);
	}

	public void Reset()
	{

	}

	public void Dispose()
	{
		if (!m_bDisposed)
		{
			m_bDisposed = true;
		}
	}
}

public class ExpatFeature
{
	public Feature Type { get; init; }
	public string Name { get; init; }
	public long Value { get; init; }

	struct SFeatureInfo
	{
		public Feature f;
		public nint n;
		public long v;
	}

	public static IEnumerable<ExpatFeature> Features { get; private set; }

	[ModuleInitializer]
	internal static unsafe void Init()
	{
		var result = new List<ExpatFeature>();

		var featureList = (SFeatureInfo*)PInvoke.XML_GetFeatureList();

		while (featureList->f != Feature.XML_FEATURE_END)
		{
			result.Add(new ExpatFeature
			{
				Type = featureList->f,
				Name = Marshal.PtrToStringAnsi(featureList->n),
				Value = featureList->v
			});

			featureList++;
		}

		Features = result.AsReadOnly();
	}
}

public enum EncodingType
{
	ASCII,
	UTF8,
	UTF16,
	ISO88591,
}

public enum ContentNodeType
{
	Text,
	Comment,
	Cdata,
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PrologHandler(nint userData, [In] nint encoding, [In] nint version, int standalone);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void StartElementHandler(nint userData, [In] nint name, [In] nint attributes);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void EndElementHandler(nint userData, [In] nint name);

// data is not 0 terminated
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CharacterDataHandler(nint userData, [In] nint data, [MarshalAs(UnmanagedType.I4)] int len);

// target and data are 0 terminated
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ProcessingInstructionHandler(nint userData, [In] nint target, [In] nint data);

// data is 0 terminated
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CommentHandler(nint userData, [In] nint data);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CdataSectionHandler(nint userData);

public static unsafe class PInvoke
{
	const string LibraryName = "libexpat";

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern nint XML_ParserCreate([In] string encoding);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void XML_ParserFree(nint parser);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void XML_SetXmlDeclHandler(nint parser, PrologHandler pHandler);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool XML_ParserReset(nint parser, [In, Optional] string encoding);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void XML_SetStartElementHandler(nint parser, [In] StartElementHandler handler);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void XML_SetEndElementHandler(nint parser, [In] EndElementHandler handler);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void XML_SetCharacterDataHandler(nint parser, [In] CharacterDataHandler handler);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void XML_SetCommentHandler(nint parser, [In] CommentHandler handler);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void XML_SetCdataSectionHandler(nint parser, [In] CdataSectionHandler start, [In] CdataSectionHandler end);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string XML_ErrorString(Error code);

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string XML_ExpatVersion();

	[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
	public static extern nint XML_GetFeatureList();
}

public enum Feature : byte
{
	XML_FEATURE_END = 0,
	XML_FEATURE_UNICODE,
	XML_FEATURE_UNICODE_WCHAR_T,
	XML_FEATURE_DTD,
	XML_FEATURE_CONTEXT_BYTES,
	XML_FEATURE_MIN_SIZE,
	XML_FEATURE_SIZEOF_XML_CHAR,
	XML_FEATURE_SIZEOF_XML_LCHAR,
	XML_FEATURE_NS,
	XML_FEATURE_LARGE_SIZE,
	XML_FEATURE_ATTR_INFO,
	XML_FEATURE_BILLION_LAUGHS_ATTACK_PROTECTION_MAXIMUM_AMPLIFICATION_DEFAULT,
	XML_FEATURE_BILLION_LAUGHS_ATTACK_PROTECTION_ACTIVATION_THRESHOLD_DEFAULT
}

public class ExpatException : Exception
{
	public Error Code { get; }

	public ExpatException(Error code) : base(PInvoke.XML_ErrorString(code))
		=> Code = code;

	public ExpatException(Error code, string message) : base(message)
	{
		Code = code;
	}
}

public enum Status
{
	XML_STATUS_ERROR = 0,
	XML_STATUS_OK = 1,
	XML_STATUS_SUSPENDED = 2
}

public enum Error
{
	XML_ERROR_NONE,
	XML_ERROR_NO_MEMORY,
	XML_ERROR_SYNTAX,
	XML_ERROR_NO_ELEMENTS,
	XML_ERROR_INVALID_TOKEN,
	XML_ERROR_UNCLOSED_TOKEN,
	XML_ERROR_PARTIAL_CHAR,
	XML_ERROR_TAG_MISMATCH,
	XML_ERROR_DUPLICATE_ATTRIBUTE,
	XML_ERROR_JUNK_AFTER_DOC_ELEMENT,
	XML_ERROR_PARAM_ENTITY_REF,
	XML_ERROR_UNDEFINED_ENTITY,
	XML_ERROR_RECURSIVE_ENTITY_REF,
	XML_ERROR_ASYNC_ENTITY,
	XML_ERROR_BAD_CHAR_REF,
	XML_ERROR_BINARY_ENTITY_REF,
	XML_ERROR_ATTRIBUTE_EXTERNAL_ENTITY_REF,
	XML_ERROR_MISPLACED_XML_PI,
	XML_ERROR_UNKNOWN_ENCODING,
	XML_ERROR_INCORRECT_ENCODING,
	XML_ERROR_UNCLOSED_CDATA_SECTION,
	XML_ERROR_EXTERNAL_ENTITY_HANDLING,
	XML_ERROR_NOT_STANDALONE,
	XML_ERROR_UNEXPECTED_STATE,
	XML_ERROR_ENTITY_DECLARED_IN_PE,
	XML_ERROR_FEATURE_REQUIRES_XML_DTD,
	XML_ERROR_CANT_CHANGE_FEATURE_ONCE_PARSING,
	XML_ERROR_UNBOUND_PREFIX,
	XML_ERROR_UNDECLARING_PREFIX,
	XML_ERROR_INCOMPLETE_PE,
	XML_ERROR_XML_DECL,
	XML_ERROR_TEXT_DECL,
	XML_ERROR_PUBLICID,
	XML_ERROR_SUSPENDED,
	XML_ERROR_NOT_SUSPENDED,
	XML_ERROR_ABORTED,
	XML_ERROR_FINISHED,
	XML_ERROR_SUSPEND_PE,
	XML_ERROR_RESERVED_PREFIX_XML,
	XML_ERROR_RESERVED_PREFIX_XMLNS,
	XML_ERROR_RESERVED_NAMESPACE_URI,
	XML_ERROR_INVALID_ARGUMENT,
	XML_ERROR_NO_BUFFER,
	XML_ERROR_AMPLIFICATION_LIMIT_BREACH
};