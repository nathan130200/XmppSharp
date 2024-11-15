using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions;

[XmppTag("time", Namespaces.EntityTime)]
public class EntityTime : Element
{
    public EntityTime() : base("time", Namespaces.EntityTime)
    {

    }

    public string? UtcTime
    {
        get => GetTag("utc");
        set
        {
            RemoveTag("utc");

            if (value != null)
                SetTag("utc", value: value);
        }
    }

    public string? TimeZone
    {
        get => GetTag("tzo");
        set
        {
            RemoveTag("tzo");

            if (value != null)
                SetTag("tzo", value: value);
        }
    }
}
