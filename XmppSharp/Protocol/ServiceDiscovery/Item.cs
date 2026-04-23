using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// This class represents the <![CDATA[<item>]]> element used in service discovery responses.
/// </summary>
[Tag("item", Namespaces.DiscoInfo)]
[Tag("item", Namespaces.DiscoItems)]
public class Item : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class.
    /// </summary>
    public Item() : base("item")
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class with the specified JID, name, and node.
    /// </summary>
    /// <param name="jid">The JID of the item.</param>
    /// <param name="name">The name of the item.</param>
    /// <param name="node">The node of the item.</param>
    public Item(Jid jid, string? name = null, string? node = null) : this()
    {
        ArgumentNullException.ThrowIfNull(jid);

        Jid = jid;

        if (name != null)
            Name = name;

        if (node != null)
            Node = node;
    }

    /// <summary>
    /// Gets or sets the JID attribute of the item, which is used to specify the Jabber ID (JID) associated with the item being described.
    /// </summary>
    /// <remarks>
    /// This attribute is required and typically contains the JID of a service, entity, or resource that is being advertised in a service discovery response.
    /// </remarks>
    public Jid Jid
    {
        get => GetAttribute("jid")!;
        set => SetAttribute("jid", value);
    }

    /// <summary>
    /// Gets or sets the name attribute of the item, which is used to provide a human-readable name for the item being described.
    /// </summary>
    /// <remarks>
    /// This attribute is optional and can be used to enhance the user experience by providing a more descriptive label for the item in service discovery responses.
    /// </remarks>
    public string? Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }

    /// <summary>
    /// Gets or sets the node attribute of the item, which is used to specify a particular node within a service discovery response. 
    /// </summary>
    /// <remarks>
    /// This attribute is optional and can be used to provide additional context or categorization for the item being described.
    /// </remarks>
    public string? Node
    {
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }
}