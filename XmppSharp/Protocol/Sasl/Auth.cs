using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// The auth element is used to initiate the authentication process.
/// </summary>
[Tag("auth", Namespaces.Sasl)]
public class Auth : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Auth"/> class.
    /// </summary>
    public Auth() : base("auth", Namespaces.Sasl)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Auth"/> class with the specified mechanism name.
    /// </summary>
    /// <param name="mechanismName">The name of the authentication mechanism.</param>
    public Auth(string mechanismName) : this()
    {
        Mechanism = mechanismName;
    }

    /// <summary>
    /// Gets or sets the name of the authentication mechanism to be used.
    /// </summary>
    public string? Mechanism
    {
        get => GetAttribute("mechanism");
        init => SetAttribute("mechanism", value);
    }
}