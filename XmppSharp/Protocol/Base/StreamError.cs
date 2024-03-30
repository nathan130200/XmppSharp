using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespaces.Stream)]
public class StreamError : Element
{
    public StreamError() : base("stream:error", Namespaces.Stream)
    {

    }

    public StreamError(StreamErrorCondition condition) : this()
    {
        Condition = condition;
    }

    public StreamError(StreamErrorCondition condition, string? text) : this(condition)
    {
        Text = text;
    }

    public StreamErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<StreamErrorCondition>())
            {
                if (HasTag(name, Namespaces.Streams))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var tag in XmppEnum.GetNames<StreamErrorCondition>())
                RemoveTag(tag, Namespaces.Streams);

            SetTag(XmppEnum.ToXmppName(value), xmlns: Namespaces.Streams);
        }
    }

    public string? Text
    {
        get => GetTag("text", Namespaces.Streams);
        set
        {
            if (value == null)
                RemoveTag("text", Namespaces.Streams);
            else
                SetTag("text", Namespaces.Streams, value);
        }
    }
}