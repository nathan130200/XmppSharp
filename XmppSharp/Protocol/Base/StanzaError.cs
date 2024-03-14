using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespace.Client)]
[XmppTag("error", Namespace.Accept)]
[XmppTag("error", Namespace.Server)]
public class StanzaError : Element
{
    public StanzaError() : base("error")
    {

    }

    public StanzaErrorType? Type
    {
        get => XmppEnum.FromXml<StanzaErrorType>(GetAttribute("type"));
        set => SetAttribute("type", value.TryGetValue(out var newValue) ? XmppEnum.ToXml(newValue) : null);
    }

    public StanzaErrorCondition? Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<StanzaErrorCondition>())
            {
                if (HasTag(name))
                    return value;
            }

            return null;
        }
        set
        {
            if (Condition.TryGetValue(out var oldValue))
                RemoveTag(XmppEnum.ToXml(oldValue));

            if (value.TryGetValue(out var newValue))
                SetTag(XmppEnum.ToXml(newValue), xmlns: Namespace.Stanzas);
        }
    }
}
