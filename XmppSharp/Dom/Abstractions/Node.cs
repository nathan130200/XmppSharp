using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom.Abstractions;

/// <summary>Represents a base class of XML node.</summary>
public abstract class Node : IXmlNode
{
	/// <inheritdoc/>
	public Element? Parent => _parent;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal Element? _parent;

	/// <summary>
	/// Clone current node recursively.
	/// </summary>
	/// <returns>New cloned instance from this node.</returns>
	public abstract Node Clone();

	/// <summary>Remove this node from its owner element.</summary>
	public void Remove() => _parent?.RemoveChild(this);

	IXmlNode IXmlNode.Clone() => Clone();

	/// <inheritdoc/>
	public abstract void WriteTo(XmlWriter writer);
}
