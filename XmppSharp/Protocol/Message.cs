using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

/// <summary>
/// Represents an XMPP message stanza, which is used for sending messages between entities in an XMPP network.
/// </summary>
[Tag("message", Namespaces.Client)]
[Tag("message", Namespaces.Component)]
[Tag("message", Namespaces.Server)]
public class Message : Stanza
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class with the default namespace.
    /// </summary>
    public Message() : base("message", DefaultNamespace)
    {

    }

    /// <summary>
    /// Gets or sets the type of the message.
    /// </summary>
    public MessageType Type
    {
        get => XmppEnum.ParseOrDefault(GetAttribute("type"), MessageType.Normal);
        set => SetAttribute("type", value.ToXmlOrDefault());
    }

    /// <summary>
    /// Gets or sets the body of the message, which contains the main content of the message.
    /// </summary>
    /// <remarks>
    /// The <![CDATA[<body>]]> element may have multiple instances to support multilingual content. This property only returns the <![CDATA[<body>]]> element that does not have an <c>xml:lang</c> attribute. If you need to access <![CDATA[<body>]]> elements with specific languages, you may need to implement additional logic to handle it accordingly.
    /// </remarks>
    public string? Body
    {
        get => GetTag("body");
        set
        {
            if (value != null)
                SetTag("body", value: value);
            else
                RemoveTag("body");
        }
    }
}
