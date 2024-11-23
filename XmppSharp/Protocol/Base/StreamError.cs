using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:error", Namespaces.Stream)]
public class StreamError : Element
{
    public StreamError() : base("stream:error", Namespaces.Stream)
    {

    }

    public StreamError(StreamErrorCondition? condition) : this()
    {
        Condition = condition;
    }

    public StreamErrorCondition? Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetXmlMapping<StreamErrorCondition>())
            {
                if (HasTag(name))
                    return value;
            }

            return default;
        }
        set
        {
            foreach (var name in XmppEnum.GetXmlNames<StreamErrorCondition>())
                RemoveTag(name);

            if (value.HasValue)
            {
                var name = XmppEnum.ToXml((StreamErrorCondition)value)!;
                SetTag(x => x.TagName = name);
            }
        }
    }

    public string? Text
    {
        get => GetTag("text", Namespaces.Streams);
        set
        {
            RemoveTag("text", Namespaces.Streams);

            if (value != null)
            {
                SetTag(x =>
                {
                    x.TagName = "text";
                    x.Namespace = Namespaces.Streams;
                    x.Value = value;
                });
            }
        }
    }
}
