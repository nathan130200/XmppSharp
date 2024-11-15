namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
    /// <summary>
    /// The "headline" category consists of services that provide real-time news or information (often but not necessarily in a message of type "headline").
    /// </summary>
    public static class Headline
    {
        const string Name = "headline";

        /// <summary>
        /// Service that notifies a user of new email messages.
        /// </summary>
        public static Identity NewMail => new(Name, "newmail");

        /// <summary>
        /// RSS notification service.
        /// </summary>
        public static Identity RSS => new(Name, "rss");

        /// <summary>
        /// Service that provides weather alerts.
        /// </summary>
        public static Identity Weather => new(Name, "weather");
    }
}