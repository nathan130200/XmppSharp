using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents a "mechanisms" element used in Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// <para>This element contains a list of SASL mechanisms supported by the server.</para>
/// </summary>
[XmppTag("mechanisms", Namespaces.Sasl)]
public class Mechanisms : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mechanisms"/> class.
    /// </summary>
    public Mechanisms() : base("mechanisms", Namespaces.Sasl)
    {

    }

    /// <summary>
    /// Gets or sets a collection of child <see cref="Mechanism"/> elements, representing the supported SASL mechanisms.
    /// <para>Setting this property replaces all existing mechanisms.</para>
    /// </summary>
    public IEnumerable<Mechanism> SupportedMechanisms
    {
        get => Children().OfType<Mechanism>();
        set
        {
            Descendants().ForEach(x => x.Remove());

            foreach (var mechanism in value)
                AddChild(mechanism);
        }
    }

    /// <summary>
    /// Adds a new <see cref="Mechanism"/> child element to the collection of supported mechanisms.
    /// </summary>
    /// <param name="mechanism">The mechanism to add.</param>
    public void AddMechanism(Mechanism mechanism)
        => AddChild(mechanism);

    /// <summary>
    /// Adds a new <see cref="Mechanism"/> child element representing the specified mechanism type.
    /// </summary>
    /// <param name="type">The type of mechanism to add.</param>
    public void AddMechanism(MechanismType type)
        => AddMechanism(mechanism: new(type));

    /// <summary>
    /// Adds a new <see cref="Mechanism"/> child element representing the specified mechanism name.
    /// </summary>
    /// <param name="name">The name of the mechanism to add.</param>
    public void AddMechanism(string name)
        => AddMechanism(mechanism: new(name));

    /// <summary>
    /// Checks if a specific SASL mechanism is supported based on its name.
    /// </summary>
    /// <param name="name">The name of the mechanism to check.</param>
    /// <returns>True if the mechanism is supported (listed in the child elements), False otherwise.</returns>
    public bool IsMechanismSupported(string name)
        => SupportedMechanisms?.Any(x => x.MechanismName == name) == true;

    /// <summary>
    /// Checks if a specific SASL mechanism is supported based on its type.
    /// This method converts the mechanism type to its corresponding XMPP name for comparison.
    /// </summary>
    /// <param name="type">The type of mechanism to check.</param>
    /// <returns>True if the mechanism is supported (listed in the child elements), False otherwise.</returns>
    public bool IsMechanismSupported(MechanismType type)
        => type != MechanismType.Unspecified && IsMechanismSupported(type.ToXmppName());
}