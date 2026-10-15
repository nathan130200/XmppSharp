using XmppSharp.Dom;

namespace XmppSharp.Protocol;

public interface IXmppSerializable
{
	Element? Serialize();
}
