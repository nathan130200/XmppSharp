using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Extensions.XEP0004;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("query", Namespaces.MucOwner)]
public class MucOwner : Element
{
    public MucOwner() : base("query", Namespaces.MucOwner)
    {

    }

    public Form? Form
    {
        get => Child<Form>();
        set
        {
            Child<Form>()?.Remove();
            AddChild(value);
        }
    }

    public Destroy? Destroy
    {
        get => Child<Destroy>();
        set
        {
            Child<Destroy>()?.Remove();
            AddChild(value);
        }
    }
}
