using System.Security.Cryptography;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents a basic XMPP stanza, incorporating common attributes and extensions.
/// </summary>
public class Stanza : DirectionalElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Stanza"/> class with the specified name, namespace, and text content.
    /// </summary>
    /// <param name="name">The name of the stanza.</param>
    /// <param name="xmlns">The optional namespace of the stanza.</param>
    /// <param name="text">The optional text content of the stanza.</param>
    public Stanza(string name, string xmlns = null, string text = null) : base(name, xmlns, text)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stanza"/> class based on another element.
    /// </summary>
    /// <param name="other">The element to copy.</param>
    protected Stanza(Element other) : base(other)
    {

    }

    /// <summary>
    /// Gets or sets the unique identifier of the stanza.
    /// </summary>
    public string Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    /// <summary>
    /// Gets or sets the type of the stanza.
    /// </summary>
    public string Type
    {
        get => GetAttribute("type");
        set => SetAttribute("type", value);
    }

    /// <summary>
    /// Gets or sets the language of the stanza.
    /// </summary>
    public string Language
    {
        get => GetAttribute("xml:lang");
        set => SetAttribute("xml:lang", value);
    }

    /// <summary>
    /// Generates a unique identifier for the stanza using a cryptographically secure random number generator.
    /// </summary>
    public void GenerateId()
    {
        Span<byte> buffer = stackalloc byte[8];
        RandomNumberGenerator.Fill(buffer);
        Id = Convert.ToHexString(buffer).ToLowerInvariant();
    }

    /// <summary>
    /// Gets or sets the error associated with the stanza, if any.
    /// </summary>
    public StanzaError Error
    {
        get => Child<StanzaError>();
        set => ReplaceChild(value);
    }
}