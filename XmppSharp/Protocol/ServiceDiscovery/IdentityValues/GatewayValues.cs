using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

/// <summary>
/// The gateway category consists of translators between Jabber/XMPP services and non-XMPP services.
/// </summary>
[XmppEnum]
public enum GatewayValues
{
    /// <summary>
    /// Gateway to AOL Instant Messenger.
    /// </summary>
    [XmppMember("aim")]
    AIM,

    /// <summary>
    /// Gateway to the Discord IM service.
    /// </summary>
    [XmppMember("discord")]
    Discord,

    /// <summary>
    /// Gateway to the Facebook IM service.
    /// </summary>
    [XmppMember("facebook")]
    Facebook,

    /// <summary>
    /// Gateway to the Gadu-Gadu IM service.
    /// </summary>
    [XmppMember("gadu-gadu")]
    GaduGadu,

    /// <summary>
    /// Gateway that provides HTTP Web Services access.
    /// </summary>
    [XmppMember("http-ws")]
    HttpWS,

    /// <summary>
    /// Gateway to ICQ.
    /// </summary>
    [XmppMember("icq")]
    ICQ,

    /// <summary>
    /// Gateway to IRC.
    /// </summary>
    [XmppMember("irc")]
    IRC,

    /// <summary>
    /// Gateway to Microsoft Live Communications Server.
    /// </summary>
    [XmppMember("lcs")]
    MSLive,

    /// <summary>
    /// Gateway to a mattermost instance IM service.
    /// </summary>
    [XmppMember("mattermost")]
    Mattermost,

    /// <summary>
    /// Gateway to the mail.ru IM service.
    /// </summary>
    [XmppMember("mrim")]
    Mailru,

    /// <summary>
    /// Gateway to MSN Messenger.
    /// </summary>
    [XmppMember("msn")]
    Msn,

    /// <summary>
    /// Gateway to the MySpace IM service.
    /// </summary>
    [XmppMember("myspaceim")]
    MySpace,

    /// <summary>
    /// Gateway to Microsoft Office Communications Server.
    /// </summary>
    [XmppMember("ocs")]
    MSOffice,

    /// <summary>
    /// Gateway to the Public Switched Telephone Network (PSTN).
    /// </summary>
    [XmppMember("pstn")]
    PSTN,

    /// <summary>
    /// Gateway to the QQ IM service.
    /// </summary>
    [XmppMember("qq")]
    QQ,

    /// <summary>
    /// Gateway to IBM Lotus Sametime.
    /// </summary>
    [XmppMember("sametime")]
    LotusSametime,

    /// <summary>
    /// Gateway to Signal IM service.
    /// </summary>
    [XmppMember("signal")]
    Signal,

    /// <summary>
    /// Gateway to SIP for Instant Messaging and Presence Leveraging Extensions (SIMPLE)
    /// </summary>
    [XmppMember("simple")]
    SIP,

    /// <summary>
    /// Gateway to the Skype service.
    /// </summary>
    [XmppMember("skype")]
    Skype,

    /// <summary>
    /// Gateway to Short Message Service.
    /// </summary>
    [XmppMember("sms")]
    SMS,

    /// <summary>
    /// Gateway to the SMTP (email) network.
    /// </summary>
    [XmppMember("smtp")]
    SMTP,

    /// <summary>
    /// Gateway to Steam IM service.
    /// </summary>
    [XmppMember("steam")]
    Steam,

    /// <summary>
    /// Gateway to the Telegram IM service.
    /// </summary>
    [XmppMember("telegram")]
    Telegram,

    /// <summary>
    /// Gateway to the Tlen IM service.
    /// </summary>
    [XmppMember("tlen")]
    Tlen,

    /// <summary>
    /// Gateway to the Xfire gaming and IM service
    /// </summary>
    [XmppMember("xfire")]
    Xfire,

    /// <summary>
    /// Gateway to another XMPP service (<b>NOT</b> via native server-to-server communication)
    /// </summary>
    [XmppMember("xmpp")]
    XMPP,

    /// <summary>
    /// Gateway to <i>Yahoo!</i> Instant Messenger
    /// </summary>
    [XmppMember("yahoo")]
    Yahoo
}