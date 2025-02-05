using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("status", Namespaces.MucUser)]
public class Status : XmppElement
{
    public Status() : base("status", Namespaces.MucUser)
    {

    }

    public Status(int code) : this()
    {
        Code = code;
    }

    public int? Code
    {
        get => this.GetAttributeInt32("code");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("code");
            else
                SetAttribute("code", (int)value);
        }
    }
}
