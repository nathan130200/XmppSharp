using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Client;

[XmppTag("bind", "urn:ietf:params:xml:ns:xmpp-bind")]
public class Bind : XElement
{
    public Bind() : base(Namespace.Bind + "bind")
    {

    }

    public Bind(string resource) : this()
        => Resource = resource;

    public Bind(Jid jid) : this()
        => Jid = jid;

    public string Resource
    {
        get => this.GetTag("resource");
        set
        {
            if (value == null)
                this.RemoveTag("resource");
            else
                this.SetTag("resource", value);
        }
    }

    public Jid Jid
    {
        get
        {
            var jid = this.GetTag("jid");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set
        {
            if (value == null)
                this.RemoveTag("jid");
            else
                this.SetTag("jid", value.ToString());
        }
    }
}
