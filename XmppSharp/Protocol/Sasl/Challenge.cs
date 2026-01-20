using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("challenge", Namespaces.Sasl)]
public sealed class Challenge : XmppElement
{
	public Challenge() : base("challenge", Namespaces.Sasl)
	{

	}
}
