﻿using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Client;

[XmppTag("session", Namespaces.Session)]
public class Session : XmppElement
{
    public Session() : base("session", Namespaces.Session)
    {

    }
}