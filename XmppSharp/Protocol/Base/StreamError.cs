using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", "http://etherx.jabber.org/streams")]
public class StreamError : XElement
{
    public StreamError() : base(Namespace.Stream + "error",
        new XAttribute(XNamespace.Xmlns + "stream", Namespace.Stream))
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
                if (this.HasTag(Namespace.Streams + name))
                    return value;
            }

            return StreamErrorCondition.UndefinedCondition;
        }
        set
        {
            this.RemoveTag(Namespace.Streams + Condition.ToXmppName());

            if (XmppEnum.IsDefined(value))
                this.SetTag(Namespace.Streams + value.ToXmppName());
        }
    }

    public string? Text
    {
        get => this.GetTag(Namespace.Streams + "text");
        set
        {
            if (value == null)
                this.RemoveTag(Namespace.Streams + "text");
            else
                this.SetTag(Namespace.Streams + "text", value);
        }
    }
}