namespace XmppSharp.Enums;

[Flags]
internal enum StreamState
{
    None,
    Read = 1 << 0,
    Write = 1 << 1,
    All = Read | Write
}