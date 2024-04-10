using System.Xml.Linq;
using XmppSharp.Attributes;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

[XmppTag("features", "http://etherx.jabber.org/streams")]
public class StreamFeatures : XElement
{
    public StreamFeatures() : base(Namespace.Stream + "features",
        new XAttribute(XNamespace.Xmlns + "stream", Namespace.Stream))
    {

    }

    public Mechanisms Mechanisms
    {
        get => this.Element<Mechanisms>();
        set
        {
            this.RemoveTag(Namespace.Sasl + "mechanisms");

            if (value != null)
                Add(value);
        }
    }

    public StartTls StartTls
    {
        get => this.Element<StartTls>();
        set
        {
            this.RemoveTag(Namespace.Tls + "starttls");

            if (value != null)
                Add(value);
        }
    }

    public bool SupportsStartTls
        => this.HasTag(Namespace.Tls + "starttls");

    public bool SupportsAuthentication
        => this.HasTag(Namespace.Sasl + "mechanisms");

    public bool SupportsBind
    {
        get => this.HasTag(Namespace.Bind + "bind");
        set
        {
            var name = Namespace.Bind + "bind";

            if (!value)
                this.RemoveTag(name);
            else
                this.SetTag(name);
        }
    }

    public bool SupportsSession
    {
        get => this.HasTag(Namespace.Session + "session");
        set
        {
            var name = Namespace.Session + "session";

            if (!value)
                this.RemoveTag(name);
            else
                this.SetTag(name);
        }
    }
}