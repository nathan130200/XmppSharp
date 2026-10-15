using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol.Extensions.XEP0085;

/// <summary>
/// An implementation of Chat State Notifications extension.
/// </summary>
[StanzaExtension("active", XMLNS_CHATSTATES)]
[StanzaExtension("composing", XMLNS_CHATSTATES)]
[StanzaExtension("paused", XMLNS_CHATSTATES)]
[StanzaExtension("inactive", XMLNS_CHATSTATES)]
[StanzaExtension("gone", XMLNS_CHATSTATES)]
public class ChatState : BaseStanzaExtension
{
	const string XMLNS_CHATSTATES = "http://jabber.org/protocol/chatstates";

	static readonly XmppEnumMapping<ChatStateType> s_TypeParser = XmppEnumMapping<ChatStateType>.Shared;

	/// <summary>
	/// Gets current chat stat.
	/// </summary>
	public ChatStateType Type { get; }

	/// <summary>
	/// Initialize a new instance of <see cref="ChatState"/> class.
	/// </summary>
	/// <param name="type">The chat state.</param>
	public ChatState(ChatStateType type)
	{
		if (!Enum.IsDefined(type))
			throw new ArgumentOutOfRangeException(nameof(type));

		Type = type;
	}

	/// <inheritdoc/>
	internal ChatState(Element element) : base(element)
	{
		Type = s_TypeParser.Parse(element.Name);
	}

	/// <inheritdoc />
	public override Element Serialize()
		=> new(s_TypeParser.GetName(Type), XMLNS_CHATSTATES);
}
