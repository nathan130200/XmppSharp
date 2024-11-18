using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0202;

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
            {
                SetTag(x =>
                {
                    x.TagName = "utc";
                    x.Value = value;
                });
            }
        }
    }

    public string? TimeZone
    {
        get => GetTag("tzo");
        set
        {
            RemoveTag("tzo");

            if (value != null)
            {
                SetTag(x =>
                {
                    x.TagName = "tzo";
                    x.Value = value;
                });
            }
        }
    }
}
