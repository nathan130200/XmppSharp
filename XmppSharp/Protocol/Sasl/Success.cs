using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("success", Namespaces.Sasl)]
public class Success : XmppElement
{
	public Success() : base("success", Namespaces.Sasl)
	{

	}
}
