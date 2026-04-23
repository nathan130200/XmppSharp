using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Xml;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// This class represents the <![CDATA[<query>]]> element used in service discovery requests and responses.
/// </summary>
[Tag("query", Namespaces.DiscoInfo)]
public class DiscoInfo : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoInfo"/> class.
    /// </summary>
    public DiscoInfo() : base("query", Namespaces.DiscoInfo)
    {

    }

    /// <summary>
    /// Gets or sets the node of the service discovery info response. 
    /// </summary>
    public string? Node
    {
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }

    /// <summary>
    /// Adds an item to the service discovery info response.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The current <see cref="DiscoInfo"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the item is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the item's namespace is not correct.</exception>
    public DiscoInfo AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.NamespaceUri != Namespaces.DiscoInfo)
            throw new ArgumentException($"The item's namespace must be '{Namespaces.DiscoInfo}'.", nameof(item));

        AddChild(item);

        return this;
    }

    /// <summary>
    /// Adds an item to the service discovery info response with the specified JID, name, and node.
    /// </summary>
    /// <param name="jid">The JID of the item.</param>
    /// <param name="name">The name of the item.</param>
    /// <param name="node">The node of the item.</param>
    /// <returns>The current <see cref="DiscoInfo"/> instance.</returns>
    public DiscoInfo AddItem(Jid jid, string? name = null, string? node = null)
    {
        var item = (Item)ElementFactory.Create("item", Namespaces.DiscoInfo);

        item.Jid = jid;
        item.Name = name;
        item.Node = node;

        AddChild(item);

        return this;
    }

    /// <summary>
    /// Gets the list of items in the service discovery info response.
    /// </summary>
    public IEnumerable<Item> Items => Elements<Item>();
}