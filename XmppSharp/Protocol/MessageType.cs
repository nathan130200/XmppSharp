using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum MessageType
{
    [XmppMember("normal")]
    Normal,

    [XmppMember("chat")]
    Chat,

    [XmppMember("error")]
    Error,

    [XmppMember("groupchat")]
    GroupChat,

    [XmppMember("headline")]
    Headline
}
