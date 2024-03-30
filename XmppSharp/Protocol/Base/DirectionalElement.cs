using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents a directional XMPP element that has sender and recipient JIDs.
/// </summary>
public class DirectionalElement : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionalElement"/> class with the specified name, namespace, and text content.
    /// </summary>
    /// <param name="name">The name of the element.</param>
    /// <param name="xmlns">The optional namespace of the element.</param>
    /// <param name="text">The optional text content of the element.</param>
    public DirectionalElement(string name, string xmlns = null, string text = null) : base(name, xmlns, text)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionalElement"/> class based on another element.
    /// </summary>
    /// <param name="other">The element to copy.</param>
    protected DirectionalElement(Element other) : base(other)
    {

    }

    /// <summary>
    /// Gets or sets the JID of the sender of the element.
    /// </summary>
    public Jid From
    {
        get
        {
            var jid = GetAttribute("from");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set => SetAttribute("from", value?.ToString());
    }

    /// <summary>
    /// Gets or sets the JID of the recipient of the element.
    /// </summary>
    public Jid To
    {
        get
        {
            var jid = GetAttribute("to");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set => SetAttribute("to", value?.ToString());
    }

    /// <summary>
    /// Switches the "from" and "to" JIDs of the element.
    /// </summary>
    public void SwitchDirection()
        => (From, To) = (To, From);
}
