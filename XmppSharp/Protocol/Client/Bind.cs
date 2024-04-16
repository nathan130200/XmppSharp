using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Client;

[XmppTag("bind", Namespace.Bind)]
public class Bind : Element
{
    public Bind() : base("bind", Namespace.Bind)
    {

    }

    public Bind(string resource) : this()
        => Resource = resource;

    public Bind(Jid jid) : this()
        => Jid = jid;

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
