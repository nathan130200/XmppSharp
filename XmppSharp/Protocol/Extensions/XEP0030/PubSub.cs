namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
    /// <summary>
    /// Services and nodes that adhere to XEP-0060.
    /// </summary>
    public static class PubSub
    {
        const string Name = "pubsub";

        /// <summary>
        /// A pubsub node of the "collection" type.
        /// </summary>
        public static Identity Collection => new(Name, "collection");

        /// <summary>
        /// A pubsub node of the "leaf" type.
        /// </summary>
        public static Identity Leaf => new(Name, "leaf");

        /// <summary>
        /// A personal eventing service that supports the publish-subscribe subset defined in XEP-0163.
        /// </summary>
        public static Identity Pep => new(Name, "pep");

        /// <summary>
        /// A pubsub service that supports the functionality defined in XEP-0060.
        /// </summary>
        public static Identity Service => new(Name, "service");
    }
}