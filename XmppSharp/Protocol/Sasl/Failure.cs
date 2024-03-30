using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("failure", Namespaces.Sasl)]
public class Failure : Element
{
    public Failure() : base("failure", Namespaces.Sasl)
    {

    }

    public Failure(FailureCondition? condition, string? message = default) : this()
    {
        Condition = condition;
        Message = message;
    }

    public FailureCondition? Condition
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetValues<FailureCondition>())
            {
                if (HasTag(key))
                    return value;
            }

            return default;
        }
        set
        {
            if (Condition.TryGetValue(out var oldValue))
                RemoveTag(XmppEnum.ToXmppName(oldValue));

            if (value.TryGetValue(out var result))
                SetTag(XmppEnum.ToXmppName(result));
        }
    }

    public string Message
    {
        get => GetTag("text");
        set => SetTag("text", value);
    }
}
