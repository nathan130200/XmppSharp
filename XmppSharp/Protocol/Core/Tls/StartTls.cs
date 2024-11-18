using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Tls;

[XmppTag("starttls", Namespaces.Tls)]
public class StartTls : Element
{
    public StartTls() : base("starttls", Namespaces.Tls)
    {

    }

    public StartTls(StartTlsPolicy policy) : this()
    {
        Policy = policy;
    }

    public StartTlsPolicy Policy
    {
        get
        {
            StartTlsPolicy result;

            if (HasTag("required", Namespaces.Tls))
                result = StartTlsPolicy.Required;
            else
                result = StartTlsPolicy.Optional;

            return result;
        }
        set
        {
            RemoveTag("optional");
            RemoveTag("required");

            string tagName = value == StartTlsPolicy.Required ? "required" : "optional";

            SetTag(x =>
            {
                x.TagName = tagName;
                x.Namespace = Namespaces.Tls;
            });
        }
    }
}
