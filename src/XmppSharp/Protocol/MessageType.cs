using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

public enum MessageType
{
    [XmppEnumMember("error")]
    Error,

    [XmppEnumMember("normal")]
    Normal,

    [XmppEnumMember("chat")]
    Chat,

    [XmppEnumMember("groupchat")]
    GroupChat,

    [XmppEnumMember("headline")]
    Headline,
}