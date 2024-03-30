using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Represents an "auth" element used in Simple Authentication and Security Layer (SASL) negotiation within XMPP.
/// <para>This element typically signifies the client's chosen authentication mechanism or provides authentication data.</para>
/// </summary>
[XmppTag("auth", Namespaces.Sasl)]
public sealed class Auth : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Auth"/> class with default properties.
    /// </summary>
    public Auth() : base("auth", Namespaces.Sasl)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Auth"/> class with the specified mechanism type.
    /// </summary>
    /// <param name="type">The type of authentication mechanism chosen by the client.</param>
    public Auth(MechanismType type) : this()
        => MechanismType = type;

    /// <summary>
    /// Initializes a new instance of the <see cref="Auth"/> class with the specified mechanism name.
    /// </summary>
    /// <param name="name">The name of the authentication mechanism chosen by the client.</param>
    public Auth(string name) : this()
        => MechanismName = name;

    /// <summary>
    /// Gets or sets the type of authentication mechanism used by the client.
    /// </summary>
    public MechanismType? MechanismType
    {
        get => XmppEnum.Parse<MechanismType>(MechanismName);
        set
        {
            if (!value.TryGetValue(out var result))
                MechanismName = null;
            else
                MechanismName = result.ToXmppName();
        }
    }

    /// <summary>
    /// Gets or sets the name of the authentication mechanism used by the client.
    /// <para>This property directly interacts with the "mechanism" attribute of the element.</para>
    /// </summary>
    public string MechanismName
    {
        get => GetAttribute("mechanism");
        set => SetAttribute("mechanism", value);
    }
}
