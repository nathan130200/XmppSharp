using System.Globalization;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("history", Namespaces.Muc)]
public class History : Element
{
    public History() : base("history", Namespaces.Muc)
    {

    }

    public int? MaxChars
    {
        get => this.GetAttribute<int>("maxchars");
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
        get => this.GetAttribute<int>("maxstanzas");
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
        get => this.GetAttribute<int>("seconds");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("seconds");
            else
                SetAttribute("seconds", (int)value);
        }
    }

    public DateTime? Since
    {
        get
        {
            if (DateTime.TryParseExact(GetAttribute("since"), Xml.XmppTimestampFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return default;
        }
        set
        {
            if (!value.HasValue)
                RemoveAttribute("since");
            else
                SetAttribute("since", value.Value, Xml.XmppTimestampFormat, CultureInfo.InvariantCulture);
        }
    }
}