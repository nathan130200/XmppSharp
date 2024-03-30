using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Disco;

[XmppEnum]
public enum IdentityCategory
{
    [XmppMember("account")]
    Account,

    [XmppMember("auth")]
    Auth,

    [XmppMember("authz")]
    Authz,

    [XmppMember("automation")]
    Automation,

    [XmppMember("client")]
    Client,

    [XmppMember("collaboration")]
    Collaboration,

    [XmppMember("component")]
    Component,

    [XmppMember("conference")]
    Conference,

    [XmppMember("directory")]
    Directory,

    [XmppMember("gateway")]
    Gateway,

    [XmppMember("headline")]
    Headline,

    [XmppMember("hierarchy")]
    Hierarchy,

    [XmppMember("proxy")]
    Proxy,

    [XmppMember("pubsub")]
    PubSub,

    [XmppMember("server")]
    Server,

    [XmppMember("store")]
    Store,
}