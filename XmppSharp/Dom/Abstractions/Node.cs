using System.Xml;

namespace XmppSharp.Dom.Abstractions;

public abstract class Node : IXmppNode
{
	public Element? Parent => _parent;

	internal Element? _parent;

	public abstract Node Clone();

	public void Remove() => _parent?.RemoveChild(this);

	IXmppNode IXmppNode.Clone() => Clone();

	public abstract void WriteTo(XmlWriter writer);
}
