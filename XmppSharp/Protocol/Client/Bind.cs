using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

/// <summary>
/// This class represents the element used in XMPP for resource binding.
/// <para>It allows clients to bind a specific resource to their JID (Jabber ID) after authentication.</para>
/// </summary>
[Tag("bind", Namespaces.Bind)]
public class Bind : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Bind"/> class.
    /// </summary>
    public Bind() : base("bind", Namespaces.Bind)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bind"/> class with a specified JID.
    /// </summary>
    /// <param name="jid">The JID to bind.</param>
    public Bind(Jid jid) : this()
    {
        Jid = jid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bind"/> class with a specified resource.
    /// </summary>
    /// <param name="resource">The resource to bind.</param>
    public Bind(string? resource) : this()
    {
        Resource = resource;
    }

    /// <summary>
    /// Gets or sets the JID binded.
    /// </summary>
    public Jid? Jid
    {
        get => GetTag("jid", Namespaces.Bind);
        set
        {
            if (value is null)
                RemoveTag("jid", Namespaces.Bind);
            else
                SetTag("jid", Namespaces.Bind, value);
        }
    }

    /// <summary>
    /// Gets or sets the resource to bind.
    /// </summary>
    public string? Resource
    {
        get => GetTag("resource", Namespaces.Bind);
        set
        {
            if (value is null)
                RemoveTag("resource", Namespaces.Bind);
            else
                SetTag("resource", Namespaces.Bind, value);
        }
    }
}