using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0202;

[XmppTag("time", Namespaces.EntityTime)]
public class EntityTime : XmppElement
{
    public EntityTime() : base("time", Namespaces.EntityTime)
    {

    }

    public DateTimeOffset? UtcTime
    {
        get
        {
            var temp = GetTag("utc");

            if (temp == null)
                return null;

            return DateTimeOffset.Parse(temp);
        }
        set
        {
            RemoveTag("utc");

            if (value != null)
                SetTag("utc", value: ((DateTimeOffset)value).ToString(Xml.XmppTimestampFormat));
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
