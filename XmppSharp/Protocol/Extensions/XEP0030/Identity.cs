using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

[XmppTag("identity", Namespaces.DiscoInfo)]
public class Identity : XmppElement
{
    public Identity() : base("identity", Namespaces.DiscoInfo)
    {

    }

    public Identity(string category, string? type) : this()
    {
        Category = category;
        Type = type;
    }

    public Identity(string category, string name, string? type) : this()
    {
        Category = category;
        ItemName = name;
        Type = type;
    }

    public string? Category
    {
        get => GetAttribute("category");
        set => SetAttribute("category", value);
    }

    public string? ItemName
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }

    public string? Type
    {
        get => GetAttribute("type");
        set => SetAttribute("type", value);
    }
}