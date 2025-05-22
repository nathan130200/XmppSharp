using System.Text;
using XmppSharp.Dom;
using XmppSharp.Net;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Sasl.Impl;

public class XmppSaslPlainHandler : XmppSaslHandler
{
    public XmppSaslPlainHandler(OutgoingXmppClientConnection connection) : base(connection)
    {
    }

    protected internal override void Init()
    {
        var buf = new StringBuilder()
            .Append('\0')
            .Append(Connection.User)
            .Append('\0')
            .Append(Connection.Password)
            .ToString()
            .GetBytes();

        var el = new Auth
        {
            Mechanism = "PLAIN",
            Value = Convert.ToBase64String(buf)
        };

        Connection.Send(el);
    }

    protected internal override SaslHandlerResult Invoke(XmppElement e)
    {
        if (e is Success)
            return SaslHandlerResult.Success;

        if (e is Failure failure)
            return SaslHandlerResult.Failure(failure.Condition, failure.Text);

        return SaslHandlerResult.Continue;
    }
}
