using System.Runtime.CompilerServices;
using XmppSharp.Dom;

namespace XmppSharp.Protocol;

/// <summary>
/// This class abstracts a stanza extension, which is an XML child element in a specific namespace inside an XMPP stanza.
/// </summary>
public abstract class BaseStanzaExtension : IXmppSerializable
{
	/// <summary>
	/// Constructor
	/// </summary>
	protected BaseStanzaExtension()
	{

	}

	/// <summary>
	/// Element constructor, parse existing XML element into concrete stanza extension.
	/// </summary>
	protected BaseStanzaExtension(Element element)
	{
		ArgumentNullException.ThrowIfNull(element);
	}

	/// <summary>
	/// Gets a XML representation of the extension.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public abstract Element Serialize();
}