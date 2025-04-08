using System.Globalization;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0203;

[XmppTag("delay", Namespaces.Delay)]
public class Delay : XmppElement
{
    public Delay() : base("delay", Namespaces.Delay)
    {

    }

    public Delay(Jid? from, DateTimeOffset? stamp) : this()
    {
        From = from;
        Stamp = stamp;
    }

    public Jid? From
    {
        get => GetAttribute("from");
        set => SetAttribute("from", value);
    }

    public DateTimeOffset? Stamp
    {
        get
        {
            var value = GetAttribute("stamp");

            if (value == null)
                return null;

            if (DateTimeOffset.TryParseExact(value, Xml.XmppTimestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return default;
        }
        set
        {
            if (!value.HasValue)
                RemoveAttribute("stamp");
            else
                SetAttribute("stamp", string.Format(CultureInfo.InvariantCulture, Xml.XmppTimestampFormatTemplate, (DateTimeOffset)value));
        }
    }
}
