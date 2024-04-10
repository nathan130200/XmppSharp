using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("failure", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Failure : XElement
{
    public Failure() : base(Namespace.Sasl + "failure")
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
                if (this.HasTag(key))
                    return value;
            }

            return default;
        }
        set
        {
            if (Condition.TryGetValue(out var oldValue))
                this.RemoveTag(oldValue.ToXmppName());

            if (value.TryGetValue(out var result))
                this.SetTag(result.ToXmppName());
        }
    }

    public string? Text
    {
        get => this.GetTag("text");
        set
        {
            if (value == null)
                this.RemoveTag("text");
            else
                this.SetTag("text", value);
        }
    }
}
