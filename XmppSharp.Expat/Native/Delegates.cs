using System.Runtime.InteropServices;

namespace XmppSharp.Expat.Native;

// 'name' is null terminated, each attribute is null terminated
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void StartElementHandler(nint userData, XML_Char* name, XML_Char** attributes);

// 'name' is null terminated
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void EndElementHandler(nint userData, XML_Char* name);

// 'data' is null terminated
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void CommentHandler(nint userData, XML_Char* data);

// 'data' is not null terminated.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void CharacterDataHandler(nint userData, XML_Char* data, int len);

// 'data' and 'target' are null terminated.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ProcessingInstructionHandler(nint userData, XML_Char* target, XML_Char* data);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void CdataSectionHandler(nint userData);

// 'version' and 'encoding are null terminated.
// 'standalone' -1 = unspecified, 0 = no, 1 = yes
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void PrologHandler(nint userData, XML_Char* version, XML_Char* encoding, int standalone);