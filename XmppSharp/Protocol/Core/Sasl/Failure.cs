using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppTag("failure", Namespaces.Sasl)]
public class Failure : XmppElement
{
    public Failure() : base("failure", Namespaces.Sasl)
    {

    }

    public Failure(FailureCondition condition, string? text = default) : this()
    {
        Condition = condition;
        Text = text;
    }

    public FailureCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetXmlMapping<FailureCondition>())
            {
                if (HasTag(name, Namespaces.Sasl))
                    return value;
            }

            return FailureCondition.Aborted;
        }
        set
        {
            foreach (var name in XmppEnum.GetXmlNames<FailureCondition>())
                RemoveTag(name, Namespaces.Sasl);

            SetTag(XmppEnum.ToXml(value)!, Namespaces.Sasl);
        }
    }

    public string? Text
    {
        get => GetTag("text", Namespaces.Sasl);
        set
        {
            RemoveTag("text", Namespaces.Sasl);

            if (value != null)
                SetTag("text", Namespaces.Sasl, value);
        }
    }
}