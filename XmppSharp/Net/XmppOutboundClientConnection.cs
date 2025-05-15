using XmppSharp.Net.Abstractions;

namespace XmppSharp.Net;

public class XmppOutboundClientConnection : XmppOutboundConnection
{
    public string User { get; set; }
    public string Resource { get; set; }
}
