using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

/// <summary>
/// Represents an XMPP presence stanza, which is used to indicate the availability and status of a user or entity in the XMPP network.
/// </summary>
[Tag("presence", Namespaces.Client)]
[Tag("presence", Namespaces.Component)]
[Tag("presence", Namespaces.Server)]
public class Presence : Stanza
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Presence"/> class with default namespace.
    /// </summary>
    public Presence() : base("presence", DefaultNamespace)
    {

    }

    /// <summary>
    /// Gets or sets the availability status of the user or entity.
    /// </summary>
    public PresenceType Type
    {
        get => XmppEnum.ParseOrDefault(GetAttribute("type"), PresenceType.Available);
        set => SetAttribute("type", value.ToXmlOrDefault());
    }

    /// <summary>
    /// Gets or sets the additional information about the availability status.
    /// </summary>
    public PresenceShow Show
    {
        get => XmppEnum.ParseOrDefault<PresenceShow>(GetTag("show"));
        set
        {
            if (Enum.IsDefined(value))
                SetTag("show", value: value.ToXml());
            else
                RemoveTag("show");
        }
    }

    /// <summary>
    /// Gets or sets the priority of the presence, which indicates the preference of this presence over others when multiple presences are available for the same user or entity.
    /// </summary>
    /// <remarks>
    /// The priority value can range from -128 to 127, where higher values indicate higher priority. If the priority is not set, it defaults to 0.
    /// </remarks>
    public sbyte? Priority
    {
        get
        {
            if (sbyte.TryParse(GetTag("priority"), out var result))
                return result;

            return default;
        }
        set
        {
            if (!value.HasValue)
                RemoveTag("priority");
            else
                SetTag("priority", value: value);
        }
    }

    /// <summary>
    /// Gets or sets the status message, which provides additional information about the user's or entity's availability or mood.
    /// <para>
    /// This is typically a free-form text string that can be displayed to other users to indicate the user's current state or activity.
    /// </para>
    /// </summary>
    public string? Status
    {
        get => GetTag("status");
        set
        {
            if (value != null)
                SetTag("status", value: value);
            else
                RemoveTag("status");
        }
    }

}