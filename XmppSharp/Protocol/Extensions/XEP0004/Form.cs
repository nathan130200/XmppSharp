using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0004;

[XmppTag("x", Namespaces.DataForms)]
public class Form : XmppElement
{
    public Form() : base("x", Namespaces.DataForms)
    {

    }

    public Form(FormType type) : this()
    {
        Type = type;
    }

    public FormType Type
    {
        get => XmppEnum.FromXmlOrDefault(GetAttribute("type"), FormType.Prompt);
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentOutOfRangeException(nameof(Type));

            SetAttribute("type", XmppEnum.ToXml(value));
        }
    }

    public IEnumerable<Item> Items
    {
        get => Elements<Item>();
        set
        {
            Elements<Item>().Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}