using System.Text;
using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("auth", Namespaces.Sasl)]
public class Auth : Element
{
    public Auth() : base("auth", Namespaces.Sasl)
    {

    }

    public MechanismType? Mechanism
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetXmlMap<MechanismType>())
            {
                if (this.HasTag(key))
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

    public string Base64Value
    {
        get => Encoding.UTF8.GetString(Convert.FromBase64String(Value));
        set => Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }
}
