using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", "jabber:client")]
[XmppTag("error", "jabber:server")]
[XmppTag("error", "jabber:component:accept")]
[XmppTag("error", "jabber:component:connect")]
public class StanzaError : XElement
{
    public StanzaError() : base(Namespace.Client + "error")
    {

    }

    public StanzaErrorType? Type
    {
        get => XmppEnum.Parse<StanzaErrorType>(this.GetAttribute("type"));
        set
        {
            if (!value.TryGetValue(out var result))
                this.RemoveAttribute("type");
            else
                this.SetAttribute("type", result.ToXmppName());
        }
    }

    public StanzaErrorCondition? Condition
    {
        get
        {
            foreach (var (tag, value) in XmppEnum.GetValues<StanzaErrorCondition>())
            {
                if (this.HasTag(Namespace.Stanzas + tag))
                    return value;
            }

            return default;
        }
        set
        {
            XmppEnum.GetNames<StanzaErrorCondition>()
                .ForEach(tag => this.RemoveTag(Namespace.Stanzas + tag));

            if (value.TryGetValue(out var result))
                this.SetTag(Namespace.Stanzas + result.ToXmppName());
        }
    }

    public string Text
    {
        get => this.GetTag(Namespace.Stanzas + "text");
        set
        {
            if (value == null)
                this.RemoveTag(Namespace.Stanzas + "text");
            else
                this.SetTag(Namespace.Stanzas + "text", value);
        }
    }
}
