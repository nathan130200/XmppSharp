using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Disco;

/// <summary>
/// Represents a service discovery information (disco#info) query element in XMPP.
/// </summary>
[XmppTag("query", Namespaces.DiscoInfo)]
public class DiscoInfo : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoInfo"/> class with no specified node.
    /// </summary>
    public DiscoInfo() : base("query", Namespaces.DiscoInfo)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoInfo"/> class with the specified node.
    /// </summary>
    /// <param name="node">The optional node to query.</param>
    public DiscoInfo(string? node) : this()
    {
        Node = node;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoInfo"/> class with the specified node, identities, and features.
    /// </summary>
    /// <param name="node">The optional node to query.</param>
    /// <param name="identities">The optional identities associated with the query.</param>
    /// <param name="features">The optional features supported by the query.</param>
    public DiscoInfo(string? node, IEnumerable<Identity>? identities, IEnumerable<Feature>? features) : this(node)
    {
        Identities = identities;
        Features = features;
    }

    /// <summary>
    /// Gets or sets the optional node to query.
    /// </summary>
    public string? Node
    {
        get => GetAttribute("node");
        set => SetAttribute("node", value);
    }

    /// <summary>
    /// Gets or sets the identities associated with the query.
    /// </summary>
    public IEnumerable<Identity> Identities
    {
        get => Children<Identity>();
        set
        {
            Children<Identity>().Remove();

            if (value != null)
            {
                foreach (var identity in value)
                    AddChild(identity);
            }
        }
    }

    /// <summary>
    /// Gets or sets the features supported by the query.
    /// </summary>
    public IEnumerable<Feature> Features
    {
        get => Children<Feature>();
        set
        {
            Children<Feature>().Remove();

            if (value != null)
            {
                foreach (var feature in value)
                    AddChild(feature);
            }
        }
    }
}
