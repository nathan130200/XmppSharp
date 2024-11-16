using XmppSharp.Attributes;
using XmppSharp.Collections;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespaces.Client)]
[XmppTag("error", Namespaces.Server)]
[XmppTag("error", Namespaces.Accept)]
[XmppTag("error", Namespaces.Connect)]
public class StanzaError : Element
{
    public StanzaError() : base("error")
    {

    }

    public StanzaErrorType? Type
    {
        get => XmppEnum.FromXmlOrDefault<StanzaErrorType>(GetAttribute("type"));
        set
        {
            if (!value.HasValue)
                RemoveAttribute("type");
            else
                SetAttribute("type", XmppEnum.ToXml(value));
        }
    }

    public string? By
    {
        get => GetAttribute("by");
        set => SetAttribute("by", value);
    }

    public StanzaErrorCondition? Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetXmlMapping<StanzaErrorCondition>())
            {
                if (HasTag(name, Namespaces.Stanzas))
                    return value;
            }

            return default;
        }
        set
        {
            foreach (var name in XmppEnum.GetNames<StanzaErrorCondition>())
                RemoveTag(name, Namespaces.Stanzas);

            if (value.HasValue)
            {
                var name = XmppEnum.ToXml((StanzaErrorCondition)value)!;
                SetTag(name, Namespaces.Stanzas);
            }
        }
    }

    public int? Code
    {
        get => this.GetAttribute<int>("code");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("code");
            else
                SetAttribute("code", (int)value);
        }
    }

    public int? CustomCode
    {
        get => this.GetAttribute<int>("custom_code");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("custom_code");
            else
                SetAttribute("custom_code", (int)value);
        }
    }

    public string? Text
    {
        get => GetTag("text", Namespaces.Stanzas);
        set
        {
            RemoveTag("text", Namespaces.Stanzas);

            if (value != null)
                SetTag("text", Namespaces.Stanzas, value);
        }
    }
}
