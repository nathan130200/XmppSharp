using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

/// <summary>
/// Represents an XMPP <![CDATA[<iq>]]> stanza, which is used for request-response interactions in the XMPP protocol.
/// </summary>
[Tag("iq", Namespaces.Client)]
[Tag("iq", Namespaces.Component)]
[Tag("iq", Namespaces.Server)]
public class Iq : Stanza
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Iq"/> class with the default namespace.
    /// </summary>
    public Iq() : base("iq", DefaultNamespace)
    {

    }

    /// <summary>
    /// Gets or sets the type of the IQ stanza.
    /// </summary>
    public IqType Type
    {
        get => XmppEnum.Parse<IqType>(GetAttribute("type"));
        set => SetAttribute("type", value.ToXml());
    }
}