﻿using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0199;

[XmppTag("ping", "urn:xmpp:ping")]
public class Ping : XmppElement
{
    public Ping() : base("ping", Namespaces.Ping)
    {

    }
}