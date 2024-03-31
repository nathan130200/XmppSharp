using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents the "stream:stream" element, the root element used to initiate an XMPP stream connection.
/// <para>This element establishes the basic communication parameters for the session, including a unique identifier, language, and XMPP protocol version.</para>
/// </summary>
[XmppTag("stream", Namespaces.Stream)]
public class StreamStream : DirectionalElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamStream"/> class.
    /// </summary>
    public StreamStream() : base("stream:stream", Namespaces.Stream)
    {

    }

    /// <summary>
    /// Gets or sets the unique identifier for the XMPP stream, assigned by the client.
    /// </summary>
    public string Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    /// <summary>
    /// Gets or sets the primary language used for the XMPP session, specified using the "xml:lang" attribute.
    /// </summary>
    public string Language
    {
        get => GetAttribute("xml:lang");
        set => SetAttribute("xml:lang", value);
    }

    /// <summary>
    /// Gets or sets the XMPP protocol version used for the session.
    /// </summary>
    public string Version
    {
        get => GetAttribute("version");
        set => SetAttribute("version", value);
    }
}
