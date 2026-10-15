using System.Xml;

namespace XmppSharp.Dom.Abstractions;

/// <summary>
/// Represents a basic XML node contract.
/// </summary>
public interface IXmppNode
{
	/// <summary>
	/// Gets the owner element of this node.
	/// </summary>
	Element? Parent { get; }

	/// <summary>
	/// Clone current element recursively.
	/// </summary>
	/// <returns>New cloned instance from this node.</returns>
	IXmppNode Clone();

	/// <summary>
	/// Serialize this node.
	/// </summary>
	/// <param name="writer">XML write for serialization.</param>
	void WriteTo(XmlWriter writer);
}
