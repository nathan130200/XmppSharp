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
                if (HasTag(name))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            Children().Where(x => x.TagName != "text")
                .Remove();

            if (Enum.IsDefined(value))
                SetTag(XmppEnum.ToXml(value));
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
                SetTag("text", value);
        }
    }
}