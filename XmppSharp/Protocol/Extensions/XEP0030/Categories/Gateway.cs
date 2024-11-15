namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
    /// <summary>
    /// The "gateway" category consists of translators between Jabber/XMPP services and non-XMPP services.
    /// </summary>
    public static class Gateway
    {
        const string Name = "gateway";

        /// <summary>
        /// Gateway to AOL Instant Messenger.
        /// </summary>
        public static Identity Aim => new(Name, "aim");

        /// <summary>
        /// Gateway to the Discord IM service.
        /// </summary>
        public static Identity Discord => new(Name, "discord");

        /// <summary>
        /// Gateway to the Facebook IM service.
        /// </summary>
        public static Identity Facebook => new(Name, "facebook");

        /// <summary>
        /// Gateway to the Gadu-Gadu IM service.
        /// </summary>
        public static Identity Gadu => new(Name, "gadu");

        /// <summary>
        /// Gateway that provides HTTP Web Services access.
        /// </summary>
        public static Identity Http => new(Name, "http");

        /// <summary>
        /// Gateway to ICQ.
        /// </summary>
        public static Identity Icq => new(Name, "icq");

        /// <summary>
        /// Gateway to IRC.
        /// </summary>
        public static Identity Irc => new(Name, "irc");

        /// <summary>
        /// Gateway to Microsoft Live Communications Server.
        /// </summary>
        public static Identity Lcs => new(Name, "lcs");

        /// <summary>
        /// Gateway to a mattermost instance IM service.
        /// </summary>
        public static Identity Mattermost => new(Name, "mattermost");

        /// <summary>
        /// Gateway to the mail.ru IM service.
        /// </summary>
        public static Identity Mrim => new(Name, "mrim");

        /// <summary>
        /// Gateway to MSN Messenger.
        /// </summary>
        public static Identity Msn => new(Name, "msn");

        /// <summary>
        /// Gateway to the MySpace IM service.
        /// </summary>
        public static Identity Myspaceim => new(Name, "myspaceim");

        /// <summary>
        /// Gateway to Microsoft Office Communications Server.
        /// </summary>
        public static Identity Ocs => new(Name, "ocs");

        /// <summary>
        /// Gateway to the Public Switched Telephone Network (PSTN).
        /// </summary>
        public static Identity Pstn => new(Name, "pstn");

        /// <summary>
        /// Gateway to the QQ IM service.
        /// </summary>
        public static Identity Qq => new(Name, "qq");

        /// <summary>
        /// Gateway to IBM Lotus Sametime.
        /// </summary>
        public static Identity Sametime => new(Name, "sametime");

        /// <summary>
        /// Gateway to Signal IM service.
        /// </summary>
        public static Identity Signal => new(Name, "signal");

        /// <summary>
        /// Gateway to SIP for Instant Messaging and Presence Leveraging Extensions (SIMPLE).
        /// </summary>
        public static Identity Simple => new(Name, "simple");

        /// <summary>
        /// Gateway to the Skype service.
        /// </summary>
        public static Identity Skype => new(Name, "skype");

        /// <summary>
        /// Gateway to Short Message Service.
        /// </summary>
        public static Identity Sms => new(Name, "sms");

        /// <summary>
        /// Gateway to the SMTP (email) network.
        /// </summary>
        public static Identity Smtp => new(Name, "smtp");

        /// <summary>
        /// Gateway to Steam IM service.
        /// </summary>
        public static Identity Steam => new(Name, "steam");

        /// <summary>
        /// Gateway to the Telegram IM service.
        /// </summary>
        public static Identity Telegram => new(Name, "telegram");

        /// <summary>
        /// Gateway to the Tlen IM service.
        /// </summary>
        public static Identity Tlen => new(Name, "tlen");

        /// <summary>
        /// Gateway to the Xfire gaming and IM service.
        /// </summary>
        public static Identity Xfire => new(Name, "xfire");

        /// <summary>
        /// Gateway to another XMPP service (NOT via native server-to-server communication) .
        /// </summary>
        public static Identity Xmpp => new(Name, "xmpp");

        /// <summary>
        /// Gateway to Yahoo! Instant Messenger.
        /// </summary>
        public static Identity Yahoo => new(Name, "yahoo");
    }
}