using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// Represents an identity element in XMPP service discovery.
/// </summary>
[XmppTag("identity", Namespaces.DiscoInfo)]
public class Identity : Element
{
    /// <summary>
    /// Creates an empty identity with default values.
    /// </summary>
    public Identity() : base("identity", Namespaces.DiscoInfo)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Identity"/> class with the specified category, type, and optional name.
    /// </summary>
    /// <param name="category">The XMPP entity category (e.g., "client", "component", "account").</param>
    /// <param name="type">The specific entity type within the category (e.g., "pc" for a client type, "pubsub" for a pubsub component type).</param>
    /// <param name="name">The optional human-readable name of the entity.</param>
    public Identity(string category, string type, string? name = default) : this(XmppEnum.ParseOrThrow<IdentityCategory>(category), type, name)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Identity"/> class with the specified XMPP enum category, type, and optional name.
    /// </summary>
    /// <param name="category">The XMPP entity category as an <see cref="IdentityCategory"/> enum value.</param>
    /// <param name="type">The specific entity type within the category (e.g., "pc" for a client type, "pubsub" for a pubsub component type).</param>
    /// <param name="name">The optional human-readable name of the entity.</param>
    public Identity(IdentityCategory category, string type, string? name = default) : this()
    {
        Category = category.ToXmppName();
        Type = type;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the XMPP entity category (e.g., "client", "component", "account").
    /// </summary>
    public string Category
    {
        get => GetAttribute("category");
        set => SetAttribute("category", value);
    }

    /// <summary>
    /// Gets the category as a strongly typed <see cref="IdentityCategory"/> enum value.
    /// </summary>
    /// <returns>The category as an <see cref="IdentityCategory"/> enum value.</returns>
    public IdentityCategory GetCategory()
        => XmppEnum.ParseOrThrow<IdentityCategory>(Category);

    /// <summary>
    /// Sets the category using a strongly typed <see cref="IdentityCategory"/> enum value.
    /// </summary>
    /// <param name="category">The category as an <see cref="IdentityCategory"/> enum value.</param>
    public void SetCategory(IdentityCategory category)
        => Category = category.ToXmppName();

    /// <summary>
    /// Gets the optional human-readable name of the entity.
    /// </summary>
    public string? Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }

    /// <summary>
    /// Gets or sets the specific entity type within the category (e.g., "pc" for a client type, "pubsub" for a pubsub component type).
    /// </summary>
    public string Type
    {
        get => GetAttribute("type");
        set => SetAttribute("type", value);
    }

    public void GetIdentityType<T>(T value) where T : struct, Enum
        => Type = value.ToXmppName();

    /// <summary>
    /// Retrieves the identity type as a strongly typed value based on the 'Category'.
    /// Utilizes XMPP enum parsing for various value types associated with different categories.
    /// </summary>
    /// <typeparam name="T">The type of the expected identity value (e.g., <see cref="AccountValues"/>, <see cref="ClientValues"/>, ...)</typeparam>
    /// <returns>The identity type as a strongly typed value, or null if not found.</returns>
    public T? GetIdentityType<T>()
    {
        object result = null;

        var str = GetAttribute("type");

        if (string.IsNullOrWhiteSpace(str))
            return default;

        var category = GetCategory();

        if (category == IdentityCategory.Account)
            result = XmppEnum.ParseOrThrow<AccountValues>(str);

        else if (category == IdentityCategory.Auth)
            result = XmppEnum.ParseOrThrow<AuthValues>(str);

        else if (category == IdentityCategory.Authz)
            result = XmppEnum.ParseOrThrow<AuthzValues>(str);

        else if (category == IdentityCategory.Automation)
            result = XmppEnum.ParseOrThrow<AutomationValues>(str);

        else if (category == IdentityCategory.Client)
            result = XmppEnum.ParseOrThrow<ClientValues>(str);

        else if (category == IdentityCategory.Collaboration)
            result = XmppEnum.ParseOrThrow<CollaborationValues>(str);

        else if (category == IdentityCategory.Component)
            result = XmppEnum.ParseOrThrow<ComponentValues>(str);

        else if (category == IdentityCategory.Conference)
            result = XmppEnum.ParseOrThrow<ConferenceValues>(str);

        else if (category == IdentityCategory.Directory)
            result = XmppEnum.ParseOrThrow<DirectoryValues>(str);

        else if (category == IdentityCategory.Gateway)
            result = XmppEnum.ParseOrThrow<GatewayValues>(str);

        else if (category == IdentityCategory.Headline)
            result = XmppEnum.ParseOrThrow<HeadlineValues>(str);

        else if (category == IdentityCategory.Hierarchy)
            result = XmppEnum.ParseOrThrow<HierarchyValues>(str);

        else if (category == IdentityCategory.Proxy)
            result = XmppEnum.ParseOrThrow<ProxyValues>(str);

        else if (category == IdentityCategory.PubSub)
            result = XmppEnum.ParseOrThrow<PubSubValues>(str);

        else if (category == IdentityCategory.Server)
            result = XmppEnum.ParseOrThrow<ServerValues>(str);

        else if (category == IdentityCategory.Store)
            result = XmppEnum.ParseOrThrow<StoreValues>(str);

        if (result is not null
            && result is T value)
            return value;

        return default;
    }

    /// <summary>
    /// Creates an identity for the "account" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The account value.</param>
    /// <param name="name">The optional name of the account.</param>
    /// <returns>A new <see cref="Identity"/> instance for the account.</returns>
    public static Identity Account(AccountValues value, string name = default)
        => new(IdentityCategory.Account, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "auth" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The authentication value.</param>
    /// <param name="name">The optional name of the authentication entity.</param>
    /// <returns>A new <see cref="Identity"/> instance for the authentication entity.</returns>
    public static Identity Auth(AuthValues value, string name = default)
        => new(IdentityCategory.Auth, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "authz" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The authorization value.</param>
    /// <param name="name">The optional name of the authorization entity.</param>
    /// <returns>A new <see cref="Identity"/> instance for the authorization entity.</returns>
    public static Identity Authz(AuthzValues value, string name = default)
        => new(IdentityCategory.Authz, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "automation" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The automation value.</param>
    /// <param name="name">The optional name of the automation entity.</param>
    /// <returns>A new <see cref="Identity"/> instance for the automation entity.</returns>
    public static Identity Automation(AutomationValues value, string name = default)
        => new(IdentityCategory.Automation, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "client" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The client value.</param>
    /// <param name="name">The optional name of the client.</param>
    /// <returns>A new <see cref="Identity"/> instance for the client.</returns>
    public static Identity Client(ClientValues value, string name = default)
        => new(IdentityCategory.Client, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "collaboration" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The collaboration value.</param>
    /// <param name="name">The optional name of the collaboration entity.</param>
    /// <returns>A new <see cref="Identity"/> instance for the collaboration entity.</returns>
    public static Identity Collaboration(CollaborationValues value, string name = default)
        => new(IdentityCategory.Collaboration, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "component" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The component value.</param>
    /// <param name="name">The optional name of the component.</param>
    /// <returns>A new <see cref="Identity"/> instance for the component.</returns>
    public static Identity Component(ComponentValues value, string name = default)
        => new(IdentityCategory.Component, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "conference" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The conference value.</param>
    /// <param name="name">The optional name of the conference.</param>
    /// <returns>A new <see cref="Identity"/> instance for the conference.</returns>
    public static Identity Conference(ConferenceValues value, string name = default)
        => new(IdentityCategory.Conference, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "directory" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The directory value.</param>
    /// <param name="name">The optional name of the directory.</param>
    /// <returns>A new <see cref="Identity"/> instance for the directory.</returns>
    public static Identity Directory(DirectoryValues value, string name = default)
        => new(IdentityCategory.Directory, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "gateway" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The gateway value.</param>
    /// <param name="name">The optional name of the gateway.</param>
    /// <returns>A new <see cref="Identity"/> instance for the gateway.</returns>
    public static Identity Gateway(GatewayValues value, string name = default)
        => new(IdentityCategory.Gateway, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "headline" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The headline value.</param>
    /// <param name="name">The optional name of the headline.</param>
    /// <returns>A new <see cref="Identity"/> instance for the headline.</returns>
    public static Identity Headline(HeadlineValues value, string name = default)
        => new(IdentityCategory.Headline, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "hierarchy" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The hierarchy value.</param>
    /// <param name="name">The optional name of the hierarchy.</param>
    /// <returns>A new <see cref="Identity"/> instance for the hierarchy.</returns>
    public static Identity Hierarchy(HierarchyValues value, string name = default)
        => new(IdentityCategory.Hierarchy, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "proxy" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The proxy value.</param>
    /// <param name="name">The optional name of the proxy.</param>
    /// <returns>A new <see cref="Identity"/> instance for the proxy.</returns>
    public static Identity Proxy(ProxyValues value, string name = default)
        => new(IdentityCategory.Proxy, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "pubsub" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The pubsub value.</param>
    /// <param name="name">The optional name of the pubsub entity.</param>
    /// <returns>A new <see cref="Identity"/> instance for the pubsub entity.</returns>
    public static Identity PubSub(PubSubValues value, string name = default)
        => new(IdentityCategory.PubSub, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "server" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The server value.</param>
    /// <param name="name">The optional name of the server.</param>
    /// <returns>A new <see cref="Identity"/> instance for the server.</returns>
    public static Identity Server(ServerValues value, string name = default)
        => new(IdentityCategory.Server, value.ToXmppName(), name);

    /// <summary>
    /// Creates an identity for the "store" category with the specified value and optional name.
    /// </summary>
    /// <param name="value">The store value.</param>
    /// <param name="name">The optional name of the store.</param>
    /// <returns>A new <see cref="Identity"/> instance for the store.</returns>
    public static Identity Store(StoreValues value, string name = default)
        => new(IdentityCategory.Store, value.ToXmppName(), name);
}