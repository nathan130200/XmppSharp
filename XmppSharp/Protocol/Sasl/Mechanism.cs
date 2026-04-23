using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// The <see cref="Mechanism"/> class represents a single authentication mechanism supported by the server. It is used in the <see cref="Mechanisms"/> element, which lists all the mechanisms that the server supports for authentication.
/// </summary>
[Tag("mechanism", Namespaces.Sasl)]
public class Mechanism : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mechanism"/> class.
    /// </summary>
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mechanism"/> class with the specified mechanism name.
    /// </summary>
    /// <param name="mechanism">The name of the authentication mechanism.</param>
    public Mechanism(string mechanism) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mechanism);

        InnerText = mechanism;
    }
}
