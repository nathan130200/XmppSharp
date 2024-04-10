using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("option", "jabber:x:data")]
public class Option : XElement
{
    public Option() : base(Namespace.DataForms + "option")
    {

    }

    public string? Label
    {
        get => this.GetAttribute("label");
        set => this.SetAttribute("label", value);
    }

    public new string Value
    {
        get => this.GetTag("value");
        set => this.SetTag("value", value);
    }
}