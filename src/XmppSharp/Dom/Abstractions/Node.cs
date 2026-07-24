using System.Xml;

namespace XmppSharp.Dom.Abstractions;

public abstract class Node : IXmlNode
{
    internal Element? _parent;

    public Element? Parent => _parent;

    public abstract Node Clone();

    public abstract void WriteTo(XmlWriter writer);

    IXmlNode IXmlNode.Clone() => Clone();

    public void Remove()
        => _parent?.RemoveChild(this);
}