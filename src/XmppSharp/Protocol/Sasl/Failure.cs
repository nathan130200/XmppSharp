using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("failure", Namespaces.Sasl)]
public class Failure : Element
{
    public FailureCondition? Condition
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetXmlMap<FailureCondition>())
            {
                if (this.HasTag(key))
                    return value;
            }

            return default;
        }
        set
        {
            if (Condition.TryUnwrap(out var old))
                this.Element(old.ToXml()).Remove();

            if (value.TryUnwrap(out var @new))
                this.SetTag(@new.ToXml());
        }
    }

    public string Message
    {
        get => this.Element("text")?.Value;
        set
        {
            var elem = this.Element("text");

            if (value == null)
                elem?.Remove();
            else
            {
                if (elem != null)
                    elem.Value = value;
                else
                    this.SetTag("text", text: value);
            }
        }
    }
}