using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol;

[XmppTag("error", Namespaces.Stream)]
public class StreamError : Element
{
    public StreamError() : base("stream:error", Namespaces.Stream)
    {

    }

    public StreamErrorCondition? Condition
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetXmlMap<StreamErrorCondition>())
            {
                if (this.HasTag(key))
                    return value;
            }

            return null;
        }
        set
        {
            RemoveAllChildren();

            if (value.TryUnwrap(out var result))
                this.SetTag(result.ToXml());
        }
    }
}
