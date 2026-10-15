namespace Jabber.Protocol.Extensions.XEP0030;

public sealed class Identity
{
    public string Category { get; }
    public string Type { get; }
    public string? Name { get; }

    public Identity(Identity other)
    {
        Category = other.Category;
        Type = other.Type;
        Name = other.Name;
    }

    public Identity(string category, string type, string? name = default)
    {
        Category = category;

        Type = type;

        Name = name;
    }
}
