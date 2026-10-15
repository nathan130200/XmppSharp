using System.Xml;

namespace XmppSharp.Dom.Abstractions;

/// <summary>
/// Represents a basic XML node contract.
/// </summary>
public interface IXmlNode
{
	/// <summary>
	/// Gets the owner element of this node.
	/// </summary>
	Element? Parent { get; }

	/// <summary>
	/// Clone current node recursively.
	/// </summary>
	/// <returns>New cloned instance from this node.</returns>
	IXmlNode Clone();

	/// <summary>
	/// Serialize this node.
	/// </summary>
	/// <param name="writer">XML write for serialization.</param>
	void WriteTo(XmlWriter writer);
}
