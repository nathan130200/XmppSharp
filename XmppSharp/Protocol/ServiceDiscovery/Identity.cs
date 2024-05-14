using XmppSharp.Attributes;
using XmppSharp.Protocol.ServiceDiscovery.IdentityValues;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("identity", Namespaces.DiscoInfo)]
public class Identity : Element
{
	public Identity() : base("identity", Namespaces.DiscoInfo)
	{

	}

	public Identity(string category, string type, string? name = default) : this(XmppEnum.ParseOrThrow<IdentityCategory>(category), type, name)
	{

	}

	public Identity(IdentityCategory category, string type, string? name = default) : this()
	{
		this.Category = category.ToXmppName();
		this.Type = type;
		this.Name = name;
	}

	public string? Category
	{
		get => this.GetAttribute("category");
		set => this.SetAttribute("category", value);
	}

	public string? Name
	{
		get => this.GetAttribute("name");
		set => this.SetAttribute("name", value);
	}

	public string? Type
	{
		get => this.GetAttribute("type");
		set => this.SetAttribute("type", value);
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