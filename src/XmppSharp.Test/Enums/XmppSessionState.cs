namespace XmppSharp.Protocol;

[Flags]
public enum XmppSessionState
{
	None = 0,
	Authenticated = 1 << 1,
	TlsStarted = 1 << 2,
	ResourceBinded = 1 << 3,
	SessionStarted = 1 << 4
}
