using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[Tag("error", Namespaces.Client)]
[Tag("error", Namespaces.Component)]
[Tag("error", Namespaces.Server)]
public sealed class Error : Element
{
    public Error() : base("error", Stanza.DefaultNamespace)
    {

    }

    public ErrorType Type
    {
        get => XmppEnum<ErrorType>.ParseOrDefault(GetAttribute("type"));
        set => SetAttribute("type", XmppEnum<ErrorType>.GetNameOrDefault(value));
    }

    public int? Code
    {
        get
        {
            if (int.TryParse(GetAttribute("code"), out var result))
                return result;

            return null;
        }

        set => SetAttribute("code", value);
    }

    public int? CustomCode
    {
        get => GetAttribute<int>("custom_code");
        set => SetAttribute("custom_code", value);
    }

    public ErrorCondition Condition
    {
        get
        {
            foreach (var (key, value) in XmppEnum<ErrorCondition>.GetMembers())
            {
                if (HasTag(key, Namespaces.Stanzas))
                    return value;
            }

            return ErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var name in XmppEnum<ErrorCondition>.GetNames())
                RemoveTag(name, Namespaces.Stanzas);

            if (Enum.IsDefined(value))
                SetTag(XmppEnum<ErrorCondition>.GetName(value), Namespaces.Stanzas);
        }
    }

    public string? Text
    {
        get => GetTag("text", Namespaces.Stanzas);

        set
        {
            if (value is null)
                RemoveTag("text", Namespaces.Stanzas);
            else
                SetTag("text", Namespaces.Stanzas, value);
        }
    }
}
