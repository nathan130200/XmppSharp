using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("abort", Namespaces.Sasl)]
public class Abort : XmppElement
{
	public Abort() : base("abort", Namespaces.Sasl)
	{

	}
}
