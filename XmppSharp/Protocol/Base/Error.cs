using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespace.Client)]
[XmppTag("error", Namespace.Accept)]
[XmppTag("error", Namespace.Server)]
public class Error : Element
{
    public Error() : base("error")
    {

    }

    public ErrorType? Type
    {
        get => XmppEnum.FromXml<ErrorType>(GetAttribute("type"));
        set => SetAttribute("type", value.TryUnwrap(out var newValue) ? XmppEnum.ToXml(newValue) : null);
    }

    public ErrorCondition? Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<ErrorCondition>())
            {
                if (HasTag(name))
                    return value;
            }

            return null;
        }
        set
        {
            if (Condition.TryUnwrap(out var oldValue))
                RemoveTag(XmppEnum.ToXml(oldValue));

            if (value.TryUnwrap(out var newValue))
                SetTag(XmppEnum.ToXml(newValue));
        }
    }
}
