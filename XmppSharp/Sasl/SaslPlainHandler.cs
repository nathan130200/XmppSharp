using System.Text;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Net;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Sasl;

public class SaslPlainHandler : SaslHandler
{
    public SaslPlainHandler(XmppClientConnection connection) : base(connection)
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

    protected internal override bool Invoke(XmppElement e)
    {
        if (e is Success)
            return true;

        if (e is Failure failure)
            throw new JabberSaslException(failure.Condition, failure.Text);

        return false;
    }
}
