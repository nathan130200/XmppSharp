using System.Xml;

namespace XmppSharp.Dom.Abstractions;

public interface IXmppNode
{
	Element? Parent { get; }

	IXmppNode Clone();

	void WriteTo(XmlWriter writer);
}
