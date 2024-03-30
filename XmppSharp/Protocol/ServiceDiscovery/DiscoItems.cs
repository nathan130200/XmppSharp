using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// Represents a service discovery request or response for items offered by an entity (e.g., a server or client).
/// </summary>
[XmppTag("query", Namespaces.DiscoItems)]
public class DiscoItems : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoItems"/> class with default properties.
    /// </summary>
    public DiscoItems() : base("query", Namespaces.DiscoItems)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoItems"/> class with the specified node identifier.
    /// </summary>
    /// <param name="node">The optional node identifier representing a specific entity or sub-collection of items.</param>
    public DiscoItems(string? node) : this()
    {
        Node = node;
    }

    /// <summary>
    /// Gets or sets the optional node identifier for this query, indicating a specific entity or sub-collection of items.
    /// </summary>
    public string? Node
    {
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }

    /// <summary>
    /// Gets or sets the collection of items discovered or advertised.
    /// </summary>
    public IEnumerable<Item> Items
    {
        get => Children<Item>();
        set
        {
            Children().Remove();

            if (value != null)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
