using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

public sealed class StreamError() : Element("stream:error", Namespaces.Stream)
{
    public StreamError(StreamErrorCondition condition, string? text = default) : this()
    {
        Condition = condition;
        Text = text;
    }

    public StreamErrorCondition Condition
    {
        get
        {
            foreach (var (key, value) in XmppEnum<StreamErrorCondition>.GetMembers())
            {
                if (HasTag(key, Namespaces.Streams))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var name in XmppEnum<StreamErrorCondition>.GetNames())
                RemoveTag(name, Namespaces.Streams);

            if (Enum.IsDefined(value))
                SetTag(XmppEnum<StreamErrorCondition>.GetName(value), Namespaces.Streams);
        }
    }

    public string? Text
    {
        get => GetTag("text", Namespaces.Streams);

        set
        {
            if (value is null)
                RemoveTag("text", Namespaces.Streams);
            else
                SetTag("text", Namespaces.Streams, value);
        }
    }
}