using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", Namespaces.Tls)]
public sealed class StartTls : Element
{
    public StartTls() : base("starttls", Namespaces.Tls)
    {

    }

    public TlsPolicy? Policy
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetXmlMap<TlsPolicy>())
            {
                if (this.Element(key) != null)
                    return value;
            }

            return default;
        }
        set
        {
            RemoveAllChildren();

            if (value.TryUnwrap(out var result))
                this.SetTag(result.ToXml());
        }
    }
}

[XmppEnum]
public enum TlsPolicy
{
    [XmppEnumMember("optional")]
    Optional,

    [XmppEnumMember("required")]
    Required
}