using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", "urn:ietf:params:xml:ns:xmpp-tls")]
public sealed class StartTls : XElement
{
    public StartTls() : base(Namespace.Tls + "starttls")
    {

    }

    public StartTls(StartTlsPolicy policy) : this()
        => Policy = policy;

    public StartTlsPolicy? Policy
    {
        get
        {
            if (this.HasTag("optional"))
                return StartTlsPolicy.Optional;

            if (this.HasTag("required"))
                return StartTlsPolicy.Required;

            return null;
        }
        set
        {
            this.RemoveTag("optional");
            this.RemoveTag("required");

            if (value.TryGetValue(out var result))
                this.SetTag(result.ToXmppName());
        }
    }
}
