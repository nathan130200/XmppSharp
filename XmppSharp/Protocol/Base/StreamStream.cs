using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream", "http://etherx.jabber.org/streams")]
public class StreamStream : DirectionalElement
{
    public StreamStream() : base(Namespace.Stream + "stream",
        new XAttribute(XNamespace.Xmlns + "stream", Namespace.Stream))
    {

    }

    public string Id
    {
        get => this.GetAttribute("id");
        set => this.SetAttribute("id", value);
    }

    public string Language
    {
        get => this.GetAttribute(XNamespace.Xml + "lang");
        set => this.SetAttribute(XNamespace.Xml + "lang", value);
    }

    public string Version
    {
        get => this.GetAttribute("version");
        set => this.SetAttribute("version", value);
    }
}
