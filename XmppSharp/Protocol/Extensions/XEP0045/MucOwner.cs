using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Extensions.XEP0004;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[Tag("query", Namespaces.MucOwner)]
public class MucOwner : XmppElement
{
	public MucOwner() : base("query", Namespaces.MucOwner)
	{

	}

	public Form? Form
	{
		get => Element<Form>();
		set
		{
			Element<Form>()?.Remove();
			AddChild(value);
		}
	}

	public Destroy? Destroy
	{
		get => Element<Destroy>();
		set
		{
			Element<Destroy>()?.Remove();
			AddChild(value);
		}
	}
}
