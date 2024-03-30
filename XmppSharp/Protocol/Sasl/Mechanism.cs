using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents a "mechanism" element used in Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// </summary>
[XmppTag("mechanism", Namespaces.Sasl)]
public class Mechanism : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mechanism"/> class with a default mechanism type of <see cref="MechanismType.Plain"/>.
    /// </summary>
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {
        MechanismType = MechanismType.Plain;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mechanism"/> class with the specified mechanism type.
    /// </summary>
    /// <param name="type">The type of mechanism to represent.</param>
    public Mechanism(MechanismType type) : this()
        => MechanismType = type;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mechanism"/> class with the specified mechanism name.
    /// </summary>
    /// <param name="name">The name of the mechanism to represent.</param>
    public Mechanism(string name) : this()
    {
        Require.NotNullOrWhiteSpace(name);
        MechanismName = name;
    }

    /// <summary>
    /// Gets or sets the mechanism type for this element.
    /// </summary>
    public MechanismType MechanismType
    {
        get => XmppEnum.ParseOrDefault(Value, MechanismType.Unspecified);
        set
        {
            if (!Enum.IsDefined(value) || value == MechanismType.Unspecified)
                MechanismName = default;
            else
                MechanismName = value.ToXmppName();
        }
    }

    /// <summary>
    /// Gets or sets the mechanism name.
    /// </summary>
    public string? MechanismName
    {
        get => Value;
        set => Value = value;
    }
}
