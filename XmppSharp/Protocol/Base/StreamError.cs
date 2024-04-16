using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:error", "http://etherx.jabber.org/streams")]
public class StreamError : Element
{
    public StreamError() : base("stream:error", Namespace.Stream)
    {

    }

    public StreamError(StreamErrorCondition condition, string text = default) : this()
    {
        Condition = condition;

        if (text != null)
            Text = text;
    }

    public StreamErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<StreamErrorCondition>())
            {
                if (HasTag(Namespace.Streams + name))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            RemoveTag(Namespace.Streams + Condition.ToXmppName());

            if (XmppEnum.IsDefined(value))
                SetTag(Namespace.Streams + value.ToXmppName());
        }
    }

    public string? Text
    {
        get => GetTag("text", Namespace.Streams);
        set
        {
            if (value == null)
                RemoveTag("text", Namespace.Streams);
            else
                SetTag("text", Namespace.Streams, value);
        }
    }
}