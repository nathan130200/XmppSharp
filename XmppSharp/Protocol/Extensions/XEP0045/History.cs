namespace XmppSharp.Protocol.Extensions.XEP0045;

public sealed record History
{
	public int? MaxChars { get; init; }
	public int? MaxStanzas { get; init; }
	public int? Seconds { get; init; }
	public DateTimeOffset? Since { get; init; }
}

[Flags]
public enum MucRoomFlags
{
	PasswordProtected = 1 << 1,
	PublicLogging = 1 << 2,
	PublicLoggingOff = 1 << 3,
	Hidden = 1 << 4,
	MembersOnly = 1 << 5,
	Moderated = 1 << 6,
	NonAnonymous = 1 << 7,
	Open = 1 << 8,
	Persistent = 1 << 9,
	Public = 1 << 10,
	SemiAnonymous = 1 << 11,
	Temporary = 1 << 12,
	Unmoderated = 1 << 13,
	Unsecured = 1 << 14,
	FullyAnonymous = 1 << 15
}

[Flags]
public enum MucUserFlags
{
	Self = 1 << 16,
	NickChanged = 1 << 17,
	Kicked = 1 << 18,
	Banned = 1 << 19,
	AffiliationChanged = 1 << 20,
	RoomDestroyed = 1 << 21,
	NickAssigned = 1 << 22,
	NewRoom = 1 << 23,
	MembershipRequired = 1 << 24,
	RoomShutdown = 1 << 25,
	AffiliationChangedWNR = 1 << 26
}