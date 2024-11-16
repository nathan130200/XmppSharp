using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0012;

[XmppTag("query", Namespaces.IqLast)]
public class LastActivity : Element
{
    public LastActivity() : base("query", Namespaces.IqLast)
    {

    }

    public LastActivity(ulong? seconds) : this()
    {
        Seconds = seconds;
    }

    public ulong? Seconds
    {
        get => this.GetAttribute<ulong>("seconds");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("seconds");
            else
                SetAttribute("seconds", (ulong)value);
        }
    }
}
