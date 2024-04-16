using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("failure", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Failure : Element
{
    public Failure() : base("failure", Namespace.Sasl)
    {

    }

    public Failure(FailureCondition condition, string text = default) : this()
    {
        Condition = condition;

        if (text != null)
            Text = text;
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
                RemoveTag(oldValue.ToXmppName());

            if (value.TryGetValue(out var result))
                SetTag(result.ToXmppName());
        }
    }

    public string? Text
    {
        get => GetTag("text");
        set
        {
            if (value == null)
                RemoveTag("text");
            else
                SetTag("text", value);
        }
    }
}
