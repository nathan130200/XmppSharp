using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", "jabber:client")]
[XmppTag("error", "jabber:server")]
[XmppTag("error", "jabber:component:accept")]
[XmppTag("error", "jabber:component:connect")]
public class StanzaError : Element
{
    public StanzaError() : base("error", Namespace.Client)
    {

    }

    public StanzaErrorType? Type
    {
        get => XmppEnum.Parse<StanzaErrorType>(GetAttribute("type"));
        set
        {
            if (!value.TryGetValue(out var result))
                RemoveAttribute("type");
            else
                SetAttribute("type", result.ToXmppName());
        }
    }

    public StanzaErrorCondition? Condition
    {
        get
        {
            foreach (var (tag, value) in XmppEnum.GetValues<StanzaErrorCondition>())
            {
                if (HasTag(Namespace.Stanzas + tag))
                    return value;
            }

            return default;
        }
        set
        {
            XmppEnum.GetNames<StanzaErrorCondition>()
                .ForEach(tag => RemoveTag(Namespace.Stanzas + tag));

            if (value.TryGetValue(out var result))
                SetTag(Namespace.Stanzas + result.ToXmppName());
        }
    }

    public string Text
    {
        get => GetTag("text", Namespace.Stanzas);
        set
        {
            if (value == null)
                RemoveTag("text", Namespace.Stanzas);
            else
                SetTag("text", Namespace.Stanzas, value);
        }
    }
}
