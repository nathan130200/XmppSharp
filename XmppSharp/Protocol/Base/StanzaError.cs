using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Extensions;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespaces.Client)]
[XmppTag("error", Namespaces.Server)]
[XmppTag("error", Namespaces.Accept)]
[XmppTag("error", Namespaces.Connect)]
public class StanzaError : XmppElement
{
    public StanzaError() : base("error")
    {

    }

    public StanzaErrorType? Type
    {
        get => XmppEnum.FromXml<StanzaErrorType>(GetAttribute("type"));
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

    public StanzaErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.XmlMapping<StanzaErrorCondition>())
            {
                if (HasTag(name, Namespaces.Stanzas))
                    return value;
            }

            return StanzaErrorCondition.UndefinedCondition;
        }
        set
        {
            XmppEnum.GetNames<StanzaErrorCondition>()
                .ForEach(x => Element(x, Namespaces.Stanzas)?.Remove());

            if (Enum.IsDefined(value) && value != StanzaErrorCondition.Unspecified)
            {
                var name = XmppEnum.ToXml(value)!;
                SetTag(name, xmlns: Namespaces.Stanzas);
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
