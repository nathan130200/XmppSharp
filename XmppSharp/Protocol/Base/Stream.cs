using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream", Namespace.Stream)]
public class Stream : Stanza
{
    public Stream() : base("stream:stream", Namespace.Stream)
    {

    }
}
