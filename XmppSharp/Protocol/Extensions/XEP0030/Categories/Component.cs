namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
    /// <summary>
    /// The "component" category consists of services that are internal to server implementations and not normally exposed outside a server.
    /// </summary>
    public static class Component
    {
        const string Name = "component";

        /// <summary>
        /// A server component that archives traffic.
        /// </summary>
        public static Identity Archive => new(Name, "archive");

        /// <summary>
        /// A server component that handles client connections.
        /// </summary>
        public static Identity C2S => new(Name, "c2s");

        /// <summary>
        /// A server component other than one of the registered types.
        /// </summary>
        public static Identity Generic => new(Name, "generic");

        /// <summary>
        /// A server component that handles load balancing.
        /// </summary>
        public static Identity Load => new(Name, "load");

        /// <summary>
        /// A server component that logs server information.
        /// </summary>
        public static Identity Log => new(Name, "log");

        /// <summary>
        /// A server component that provides presence information.
        /// </summary>
        public static Identity Presence => new(Name, "presence");

        /// <summary>
        /// A server component that handles core routing logic.
        /// </summary>
        public static Identity Router => new(Name, "router");

        /// <summary>
        /// A server component that handles server connections.
        /// </summary>
        public static Identity S2S => new(Name, "s2s");

        /// <summary>
        /// A server component that manages user sessions.
        /// </summary>
        public static Identity SM => new(Name, "sm");

        /// <summary>
        /// A server component that provides server statistics.
        /// </summary>
        public static Identity Stats => new(Name, "stats");
    }
}