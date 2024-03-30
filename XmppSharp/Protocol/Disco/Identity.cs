using XmppSharp.Attributes;
using XmppSharp.Protocol.Disco.IdentityValues;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Disco;

[XmppTag("identity", Namespaces.DiscoInfo)]
public class Identity : Element
{
    public Identity() : base("identity", Namespaces.DiscoInfo)
    {

    }

    public Identity(string category, string type, string? name = default) : this(XmppEnum.ParseOrThrow<IdentityCategory>(category), type, name)
    {

    }

    public Identity(IdentityCategory category, string type, string name = default) : this()
    {
        Category = category.ToXmppName();
        Type = type;
        Name = name;
    }

    public string Category
    {
        get => GetAttribute("category");
        set => SetAttribute("category", value);
    }

    public IdentityCategory GetIdentityCategory()
        => XmppEnum.ParseOrThrow<IdentityCategory>(Category);

    public void SetCategory(IdentityCategory category)
        => Category = category.ToXmppName();

    public string? Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }

    public string Type
    {
        get => GetAttribute("type");
        set => SetAttribute("type", value);
    }

    public T? GetIdentityType<T>()
    {
        object result = null;

        var str = GetAttribute("type");

        if (string.IsNullOrWhiteSpace(str))
            return default;

        var category = GetIdentityCategory();

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

    public static Identity Account(AccountValues value, string name = default)
        => new(IdentityCategory.Account, value.ToXmppName(), name);

    public static Identity Auth(AuthValues value, string name = default)
        => new(IdentityCategory.Auth, value.ToXmppName(), name);

    public static Identity Authz(AuthzValues value, string name = default)
        => new(IdentityCategory.Authz, value.ToXmppName(), name);

    public static Identity Automation(AutomationValues value, string name = default)
        => new(IdentityCategory.Automation, value.ToXmppName(), name);

    public static Identity Client(ClientValues value, string name = default)
        => new(IdentityCategory.Client, value.ToXmppName(), name);

    public static Identity Collaboration(CollaborationValues value, string name = default)
        => new(IdentityCategory.Collaboration, value.ToXmppName(), name);

    public static Identity Component(ComponentValues value, string name = default)
        => new(IdentityCategory.Component, value.ToXmppName(), name);

    public static Identity Conference(ConferenceValues value, string name = default)
        => new(IdentityCategory.Conference, value.ToXmppName(), name);

    public static Identity Directory(DirectoryValues value, string name = default)
        => new(IdentityCategory.Directory, value.ToXmppName(), name);

    public static Identity Gateway(GatewayValues value, string name = default)
        => new(IdentityCategory.Gateway, value.ToXmppName(), name);

    public static Identity Headline(HeadlineValues value, string name = default)
        => new(IdentityCategory.Headline, value.ToXmppName(), name);

    public static Identity Hierarchy(HierarchyValues value, string name = default)
        => new(IdentityCategory.Hierarchy, value.ToXmppName(), name);

    public static Identity Proxy(ProxyValues value, string name = default)
        => new(IdentityCategory.Proxy, value.ToXmppName(), name);

    public static Identity PubSub(PubSubValues value, string name = default)
        => new(IdentityCategory.PubSub, value.ToXmppName(), name);

    public static Identity Server(ServerValues value, string name = default)
        => new(IdentityCategory.Server, value.ToXmppName(), name);

    public static Identity Store(StoreValues value, string name = default)
        => new(IdentityCategory.Store, value.ToXmppName(), name);
}