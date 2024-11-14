using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.MultiUserChat;

[XmppTag("status", Namespaces.MucUser)]
public class Status : Element
{
    public Status() : base("status", Namespaces.MucUser)
    {

    }

    public Status(int code) : this()
    {
        Children<Actor>();

        Code = code;
    }

    public int Code
    {
        get => this.GetAttribute("code", 0);
        set => SetAttribute("code", value);
    }
}
