using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.MultiUserChat;

[XmppTag("x", Namespaces.Muc)]
public class Muc : Element
{
    public Muc() : base("x", Namespaces.Muc)
    {

    }

    public string? Password
    {
        get => GetTag("password");
        set
        {
            RemoveTag("password");

            if (value != null)
                SetTag("password", value);
        }
    }
}
