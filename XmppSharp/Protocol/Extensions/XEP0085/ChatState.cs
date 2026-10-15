using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol.Extensions.XEP0085;

[StanzaExtension("active", XMLNS_CHATSTATES)]
[StanzaExtension("composing", XMLNS_CHATSTATES)]
[StanzaExtension("paused", XMLNS_CHATSTATES)]
[StanzaExtension("inactive", XMLNS_CHATSTATES)]
[StanzaExtension("gone", XMLNS_CHATSTATES)]
public class ChatState : StanzaExtension
{
	const string XMLNS_CHATSTATES = "http://jabber.org/protocol/chatstates";

	static readonly XmppEnumMapping<ChatStateType> s_TypeParser = XmppEnumMapping<ChatStateType>.Shared;

	public ChatStateType Type { get; }

	public ChatState(ChatStateType type)
	{
		if (!Enum.IsDefined(type))
			throw new ArgumentOutOfRangeException(nameof(type));

		Type = type;
	}

	internal ChatState(Element element) : base(element)
	{
		Type = s_TypeParser.Parse(element.Name);
	}

	public override Element Serialize()
		=> new(s_TypeParser.GetName(Type), XMLNS_CHATSTATES);
}
