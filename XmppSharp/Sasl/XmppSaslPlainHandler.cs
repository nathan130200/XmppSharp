﻿using System.Text;
using XmppSharp.Net;
using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp.Sasl;

public class XmppSaslPlainHandler : XmppSaslMechanismHandler
{
    public XmppSaslPlainHandler(XmppClientConnection connection) : base(connection)
    {
    }

    protected internal override void Init()
    {
        var buf = new StringBuilder()
            .Append('\0')
            .Append(Connection.Options.Jid.Local)
            .Append('\0')
            .Append(Connection.Options.Password)
            .ToString()
            .GetBytes();

        var tag = new Auth
        {
            Mechanism = "PLAIN",
            Value = Convert.ToBase64String(buf)
        };

        Connection.Send(tag);
    }
}
