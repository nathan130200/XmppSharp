using System.Runtime.CompilerServices;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents an element that has a direction, such as a message or presence stanza.
/// </summary>
public abstract class DirectionalElement : Element
{
    /// <summary>
    /// Gets or sets the JID of the sender of the stanza.
    /// </summary>
    public Jid? From
    {
        get => GetAttribute("from");
        set => SetAttribute("from", value);
    }

    /// <summary>
    /// Gets or sets the JID of the recipient of the stanza.
    /// </summary>
    public Jid? To
    {
        get => GetAttribute("to");
        set => SetAttribute("to", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionalElement"/> class with the specified tag name, namespace URI, and value.
    /// </summary>
    /// <param name="tagName">The name of the XML tag.</param>
    /// <param name="namespaceURI">The namespace URI of the XML element.</param>
    /// <param name="value">The value of the XML element.</param>
    public DirectionalElement(string tagName, string? namespaceURI = null, object? value = null)
        : base(tagName, namespaceURI, value)
    {
    }

    /// <summary>
    /// Switches the direction of the stanza by swapping the values of the "from" and "to" attributes.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void SwitchDirection()
        => (From, To) = (To, From);
}