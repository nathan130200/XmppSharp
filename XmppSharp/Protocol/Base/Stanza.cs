using System.Runtime.CompilerServices;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Base class for stanzas (message, presence, iq).
/// </summary>
/// <param name="tagName">The name of the XML tag for the stanza.</param>
/// <param name="namespaceURI">The namespace URI for the stanza.</param>
/// <param name="value">The value of the stanza.</param>
public abstract class Stanza(string tagName, string? namespaceURI = null, object? value = null)
    : DirectionalElement(tagName, namespaceURI, value)
{
    static readonly AsyncLocal<string?> s_DefaultNamespace = new();

    //globally define default namespace of an stanza element (apply both for IQ, mEssage and Presence)

    /// <summary>
    /// Gets or sets the default namespace for stanzas. If not set, it defaults to <see cref="Namespaces.Client" />.
    /// </summary>
    public static string DefaultNamespace
    {
        get => s_DefaultNamespace.Value ?? Namespaces.Client;
        set => s_DefaultNamespace.Value = value;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the stanza.
    /// </summary>
    /// <remarks>This attribute is used to correlate requests and responses in XMPP communication.</remarks>
    public string? Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    /// <summary>
    /// Gets or sets the error information associated with the stanza, if any.
    /// </summary>
    public Error? Error
    {
        get => Element<Error>();
        set
        {
            RemoveTag("error", NamespaceUri);

            AddChild(value);
        }
    }

    /// <summary>
    /// Generates a new unique identifier for the stanza.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void GenerateId()
    {
        Id = Guid.NewGuid().ToString("n");
    }
}
