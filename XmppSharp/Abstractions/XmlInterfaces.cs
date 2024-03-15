using System.Xml;

namespace XmppSharp.Abstractions;

public interface IXmlSerializer
{
    void Serialize(XmlWriter writer);
}

public interface IXmlDeserializer
{
    void Deserialize(XmlReader reader);
}
