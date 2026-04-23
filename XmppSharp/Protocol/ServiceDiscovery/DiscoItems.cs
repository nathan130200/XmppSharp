using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Xml;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// This class represents the <![CDATA[<query>]]> element used in service discovery item requests and responses.
/// </summary>
[Tag("query", Namespaces.DiscoItems)]
public class DiscoItems : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoItems"/> class.
    /// </summary>
    public DiscoItems() : base("query", Namespaces.DiscoItems)
    {

    }

    /// <summary>
    /// Gets or sets the node of the service discovery items response. 
    /// </summary>
    public string? Node
    {
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }

    /// <summary>
    /// Adds an item to the service discovery items list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The current <see cref="DiscoItems"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the item is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the item's namespace is not correct.</exception>
    public DiscoItems AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.NamespaceUri != Namespaces.DiscoItems)
            throw new ArgumentException($"The item must be in the '{Namespaces.DiscoItems}' namespace.", nameof(item));

        AddChild(item);

        return this;
    }

    /// <summary>
    /// Adds an item to the service discovery items list.
    /// </summary>
    /// <param name="jid">The JID of the item.</param>
    /// <param name="name">The name of the item.</param>
    /// <param name="node">The node of the item.</param>
    /// <returns>The current <see cref="DiscoItems"/> instance.</returns>
    public DiscoItems AddItem(Jid jid, string? name = null, string? node = null)
    {
        ArgumentNullException.ThrowIfNull(jid);

        var item = (Item)ElementFactory.Create("item", Namespaces.DiscoItems);

        item.Jid = jid;
        item.Name = name;
        item.Node = node;

        AddChild(item);

        return this;
    }

    /// <summary>
    /// Gets the list of items in the service discovery items response.
    /// </summary>
    public IEnumerable<Item> Items => Elements<Item>();
}
