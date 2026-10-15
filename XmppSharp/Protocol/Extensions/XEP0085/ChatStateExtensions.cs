namespace XmppSharp.Protocol.Extensions.XEP0085;

public static class ChatStateExtensions
{
	public static ChatStateType GetChatState(this Message stanza)
	{
		ChatStateType result = 0;

		foreach (var ext in stanza.GetExtensions<ChatState>())
			result |= ext.Type;

		return result;
	}
}
