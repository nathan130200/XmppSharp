using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

/// <summary>
/// Represents a "bind" element used within XMPP for resource binding.
/// <para>This class facilitates the binding of a resource to a JID (Jabber ID), allowing multiple resources to connect to the same account.</para>
/// </summary>
[XmppTag("bind", Namespaces.Bind)]
public class Bind : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Bind"/> class.
    /// </summary>
    public Bind() : base("bind", Namespaces.Bind)
    {

    }

    /// <summary>
    /// Creates a "bind" element with a specified resource for binding.
    /// </summary>
    /// <param name="resource">The resource to bind.</param>
    public Bind(string resource) : this()
        => Resource = resource;

    /// <summary>
    /// Creates a "bind" element with a specified JID for binding.
    /// </summary>
    /// <param name="jid">The JID to bind.</param>
    public Bind(Jid jid) : this()
        => Jid = jid;

    /// <summary>
    /// Gets or sets the resource to be bound.
    /// </summary>
    public string Resource
    {
        get => GetTag("resource");
        set
        {
            if (value == null)
                RemoveTag("resource");
            else
                SetTag("resource", value);
        }
    }

    /// <summary>
    /// Gets or sets the JID to be bound.
    /// </summary>
    public Jid Jid
    {
        get
        {
            var jid = GetTag("jid");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set
        {
            if (value == null)
                RemoveTag("jid");
            else
                SetTag("jid", value.ToString());
        }
    }
}
