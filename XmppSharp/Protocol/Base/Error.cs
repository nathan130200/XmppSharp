using Jabber.Abstractions;
using Jabber.Attributes;
using Jabber.Dom;
using Jabber.Serialization;

namespace Jabber.Protocol.Base;

[StanzaExtension("error", Namespaces.Client)]
[StanzaExtension("error", Namespaces.Server)]
[StanzaExtension("error", Namespaces.Component)]
public sealed class Error : StanzaExtension
{
    static readonly XmppEnumMapping<ErrorType> s_ErrorTypeParser
        = XmppEnumMapping<ErrorType>.Shared;

    static readonly XmppEnumMapping<ErrorCondition> s_ErrorConditionParser
        = XmppEnumMapping<ErrorCondition>.Shared;

    public Error()
    {
        Type = ErrorType.Modify;

        Condition = ErrorCondition.UndefinedCondition;
    }

    internal Error(Element element) : base(element)
    {
        Type = s_ErrorTypeParser.Parse(element.Attribute("type"));

        By = element.Attribute("by");
        Code = element.Attribute("code");
        CustomCode = element.Attribute("custom_code");

        Text = element.GetTag("text", Namespaces.Stanzas);

        foreach (var tag in element.Elements())
        {
            if (tag.Namespace != Namespaces.Stanzas) continue;

            foreach (var (name, value) in s_ErrorConditionParser.NameToValue)
            {
                if (element.HasTag(name, Namespaces.Stanzas))
                {
                    Condition = value;
                    break;
                }
            }
        }
    }

    public ErrorType Type
    {
        get;
        set;
    }

    public ErrorCondition Condition
    {
        get;
        set;
    }

    public string? Text
    {
        get;
        set;
    }

    public int? Code
    {
        get;
        set;
    }

    public string? By
    {
        get;
        set;
    }

    public int? CustomCode
    {
        get;
        set;
    }

    public override Element Serialize()
    {
        var result = new Element("error");

        result.SetAttribute("by", By);
        result.SetAttribute("type", s_ErrorTypeParser.GetName(Type));
        result.SetAttribute("code", Code);
        result.SetAttribute("custom_code", CustomCode);

        if (s_ErrorConditionParser.GetNameOrDefault(Condition) is string condition)
            result.SetTag(condition, Namespaces.Stanzas);

        if (Text is not null)
            result.SetTag("text", Namespaces.Stanzas, Text);

        return result;
    }
}