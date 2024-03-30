using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.ServiceDiscovery;

/// <summary>
/// Represents a feature in a service discovery information (disco#info) response.
/// </summary>
[XmppTag("feature", Namespaces.DiscoInfo)]
public class Feature : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Feature"/> class with no feature name.
    /// </summary>
    public Feature() : base("feature", Namespaces.DiscoInfo)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Feature"/> class with the specified feature name.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    public Feature(string featureName) : this()
    {
        Name = featureName;
    }

    /// <summary>
    /// Gets or sets the name of the feature.
    /// </summary>
    public string Name
    {
        get => GetAttribute("var");
        set => SetAttribute("var", value);
    }
}