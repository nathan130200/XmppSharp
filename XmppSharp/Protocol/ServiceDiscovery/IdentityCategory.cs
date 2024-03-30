using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// Represents the different categories of XMPP entities used in service discovery, refer to the <a href="https://xmpp.org/registrar/disco-categories.html">XMPP Registrar</a> documentation.
/// </summary>
[XmppEnum]
public enum IdentityCategory
{
    /// <summary>
    /// Denotes an account entity, typically representing a user or service with an XMPP address
    /// that can authenticate and interact with the XMPP network.
    /// </summary>
    [XmppMember("account")]
    Account,

    /// <summary>
    /// Denotes an authentication entity, likely responsible for handling user logins and password verification.
    /// </summary>
    [XmppMember("auth")]
    Auth,

    /// <summary>
    /// Denotes an authorization entity, responsible for managing access control and permissions within the XMPP network.
    /// </summary>
    [XmppMember("authz")]
    Authz,

    /// <summary>
    /// Denotes an automation entity, likely representing a bot or scripted program interacting with the XMPP network.
    /// </summary>
    [XmppMember("automation")]
    Automation,

    /// <summary>
    /// Denotes a client entity, typically representing a software application connecting to the XMPP network
    /// to communicate with other users or services.
    /// </summary>
    [XmppMember("client")]
    Client,

    // <summary>
    /// Denotes a collaboration entity, likely involved in features like group chat, shared workspaces, or other collaborative activities.
    /// </summary>
    [XmppMember("collaboration")]
    Collaboration,

    /// <summary>
    /// Denotes a component entity, a lightweight program or service attached to a server and extending its functionality.
    /// </summary>
    [XmppMember("component")]
    Component,

    /// <summary>
    /// Denotes a conference entity, likely related to multi-user chat rooms or teleconferencing features.
    /// </summary>
    [XmppMember("conference")]
    Conference,

    /// <summary>
    /// Denotes a directory entity, consists of information retrieval services that enable users to search online directories or otherwise be informed about the existence of other XMPP entities
    /// </summary>
    [XmppMember("directory")]
    Directory,

    /// <summary>
    /// Denotes a gateway entity, acting as a bridge between the XMPP network and another communication protocol.
    /// </summary>
    [XmppMember("gateway")]
    Gateway,

    /// <summary>
    /// Denotes a headline entity, consists of services that provide real-time news or information.
    /// </summary>
    [XmppMember("headline")]
    Headline,

    /// <summary>
    /// Denotes a hierarchy entity, related to representing user or service relationships within a structure.
    /// </summary>
    [XmppMember("hierarchy")]
    Hierarchy,

    /// <summary>
    /// Denotes a proxy entity, acting as an intermediary between XMPP clients or servers.
    /// </summary>
    [XmppMember("proxy")]
    Proxy,

    /// <summary>
    /// Denotes a pubsub entity, likely related to the Publish-Subscribe protocol for message distribution within XMPP.
    /// </summary>
    [XmppMember("pubsub")]
    PubSub,

    /// <summary>
    /// Denotes a server entity, representing a core XMPP server providing communication services for clients and components.
    /// </summary>
    [XmppMember("server")]
    Server,

    /// <summary>
    /// Denotes a store entity, related to data storage or retrieval functionalities within the XMPP network.
    /// </summary>
    [XmppMember("store")]
    Store,
}