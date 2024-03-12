using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespace.Stream)]
public class StreamError : Element
{
    public StreamError() : base("stream:error", Namespace.Stream)
    {

    }

    public StreamError(StreamErrorCondition condition) : this()
        => Condition = condition;

    public StreamErrorCondition Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<StreamErrorCondition>())
            {
                if (HasTag(name, Namespace.Streams))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            foreach (var tag in XmppEnum.GetNames<StreamErrorCondition>())
                RemoveTag(tag, Namespace.Stanzas);

            SetTag(XmppEnum.ToXml(value), xmlns: Namespace.Streams);
        }
    }

    public string Text
    {
        get => GetTag("text");
        set
        {
            if (value == null)
                RemoveTag("text");
            else
                SetTag("text", Namespace.Streams, value);
        }
    }
}