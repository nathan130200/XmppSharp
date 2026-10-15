using XmppSharp.Dom;

namespace XmppSharp.Protocol;

/// <summary>
/// Represents the contract of an object that can be serialized to an XML element.
/// </summary>
public interface IXmppSerializable
{
	/// <summary>
	/// Gets XML representation of current object.
	/// </summary>
	Element Serialize();
}
