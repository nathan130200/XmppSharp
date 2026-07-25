using System.Xml;

namespace XmppSharp.Dom.Abstractions;

public interface IXmlNode
{
	Element? Parent { get; }

	IXmlNode Clone();

	void WriteTo(XmlWriter writer);

	void Remove();
}
