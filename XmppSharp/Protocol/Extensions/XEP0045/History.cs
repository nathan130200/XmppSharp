namespace XmppSharp.Protocol.Extensions.XEP0045;

/// <summary>
/// History management in a MUC room.
/// </summary>
public sealed record History
{
	/// <summary>
	/// Limit the total number of characters in the history.
	/// </summary>
	public int? MaxChars { get; init; }

	/// <summary>
	/// Limit the total number of messages in the history.
	/// </summary>
	public int? MaxStanzas { get; init; }

	/// <summary>
	/// Send only the messages received in the last seconds.
	/// </summary>
	public int? Seconds { get; init; }

	/// <summary>
	/// Send only the messages received since the datetime specified.
	/// </summary>
	public DateTimeOffset? Since { get; init; }
}