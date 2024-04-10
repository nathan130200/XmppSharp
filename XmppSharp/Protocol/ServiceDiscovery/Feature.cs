using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("feature", "http://jabber.org/protocol/disco#info")]
public class Feature : XElement
{
    public Feature() : base(Namespace.DiscoInfo + "feature")
    {

    }

    public Feature(string featureName) : this()
    {
        Name = featureName;
    }

    public new string Name
    {
        get => this.GetAttribute("var");
        set => this.SetAttribute("var", value);
    }
}