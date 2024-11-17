using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("decline", Namespaces.MucUser)]
public class Decline : DirectionalElement
{
    public Decline() : base("decline", Namespaces.MucUser)
    {

    }

    public string? Reason
    {
        get => GetTag("reason");
        set
        {
            RemoveTag("reason");

            if (value != null)
                SetTag("reason", value: value);
        }
    }
}
