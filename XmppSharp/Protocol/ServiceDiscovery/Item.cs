using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// Represents an item discovered or advertised through service discovery (DISCO) in XMPP.
/// </summary>
[XmppTag("item", Namespaces.DiscoItems)]
public class Item : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class with default values.
    /// </summary>
    public Item() : base("item", Namespaces.DiscoItems)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class with the specified JID and name.
    /// </summary>
    /// <param name="jid">The optional JID (Jabber ID) associated with the item.</param>
    /// <param name="name">The optional human-readable name of the item.</param>
    public Item(Jid? jid, string? name) : this()
    {
        Jid = jid;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the JID (Jabber ID) associated with the item.
    /// </summary>
    public Jid? Jid
    {
        get => GetAttribute("jid");
        set => SetAttribute("jid", value);
    }

    /// <summary>
    /// Gets or sets the human-readable name of the item.
    /// </summary>
    public string? Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }
}