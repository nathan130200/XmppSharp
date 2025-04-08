using System.Globalization;
using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Extensions;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("history", Namespaces.Muc)]
public class History : XmppElement
{
    public History() : base("history", Namespaces.Muc)
    {

    }

    public int? MaxChars
    {
        get => this.GetAttributeInt32("maxchars");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("maxchars");
            else
                SetAttribute("maxchars", (int)value);
        }
    }

    public int? MaxStanzas
    {
        get => this.GetAttributeInt32("maxstanzas");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("maxstanzas");
            else
                SetAttribute("maxstanzas", (int)value);
        }
    }

    public int? Seconds
    {
        get => this.GetAttributeInt32("seconds");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("seconds");
            else
                SetAttribute("seconds", (int)value);
        }
    }

    public DateTimeOffset? Since
    {
        get
        {
            if (!HasAttribute("since"))
                return default;

            if (DateTimeOffset.TryParseExact(GetAttribute("since"), Xml.XmppTimestampFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return default;
        }
        set
        {
            if (!value.HasValue)
                RemoveAttribute("since");
            else
                SetAttribute("since", string.Format(CultureInfo.InvariantCulture, Xml.XmppTimestampFormatTemplate, (DateTimeOffset)value));
        }
    }
}