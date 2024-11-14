using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0004;

[XmppEnum]
public enum FieldType
{
    [XmppMember("boolean")]
    Boolean,

    [XmppMember("fixed")]
    Fixed,

    [XmppMember("hidden")]
    Hidden,

    [XmppMember("jid-multi")]
    JidMulti,

    [XmppMember("jid-single")]
    JidSingle,

    [XmppMember("list-multi")]
    ListMulti,

    [XmppMember("list-single")]
    ListSingle,

    [XmppMember("text-multi")]
    TextMulti,

    [XmppMember("text-private")]
    TextPrivate,

    [XmppMember("text-single")]
    TextSingle,
}
