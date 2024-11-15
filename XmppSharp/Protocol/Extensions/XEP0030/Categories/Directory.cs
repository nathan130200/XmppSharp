namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
    /// <summary>
    /// The "directory" category consists of information retrieval services that enable users to search online directories or otherwise be informed about the existence of other XMPP entities.
    /// </summary>
    public static class Directory
    {
        const string Name = "directory";

        /// <summary>
        /// A directory of chatrooms.
        /// </summary>
        public static Identity ChatRoom => new(Name, "chatroom");

        /// <summary>
        /// A directory that provides shared roster groups.
        /// </summary>
        public static Identity Group => new(Name, "group");

        /// <summary>
        /// A directory of end users (e.g., JUD)
        /// </summary>
        public static Identity User => new(Name, "user");

        /// <summary>
        /// A directory of waiting list entries.
        /// </summary>
        public static Identity WaitingList => new(Name, "waitinglist");
    }
}