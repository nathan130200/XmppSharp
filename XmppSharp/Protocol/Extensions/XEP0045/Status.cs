using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Extensions;

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
        get => this.GetAttribute<int>("code");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("code");
            else
                SetAttribute("code", (int)value);
        }
    }
}
