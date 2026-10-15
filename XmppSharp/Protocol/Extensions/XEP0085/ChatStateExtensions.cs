namespace XmppSharp.Protocol.Extensions.XEP0085;

/// <summary>
/// A collection of <see cref="ChatState"/> helper methods.
/// </summary>
public static class ChatStateExtensions
{
	/// <summary>
	/// Get combined chat states in provided message.
	/// </summary>
	/// <param name="stanza">The message.</param>
	public static ChatStateType GetChatState(this Message stanza)
	{
		ChatStateType result = 0;

		foreach (var ext in stanza.GetExtensions<ChatState>())
			result |= ext.Type;

		return result;
	}
}
