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

    public StanzaErrorType Type
    {
        get => XmppEnum.FromXml(GetAttribute("type"), StanzaErrorType.Cancel);
        set => SetAttribute("type", XmppEnum.ToXml(value));
    }

    public StanzaErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<StanzaErrorCondition>())
            {
                if (HasTag(name))
                    return value;
            }

            return StanzaErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var tag in XmppEnum.GetNames<StanzaErrorCondition>())
                RemoveTag(tag, Namespace.Stanzas);

            SetTag(XmppEnum.ToXml(value), xmlns: Namespace.Stanzas);
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
