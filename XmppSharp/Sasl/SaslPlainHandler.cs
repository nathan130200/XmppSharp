using System.Text;
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
            .Append(Connection.Options.Username)
            .Append('\0')
            .Append(Connection.Options.Password)
            .ToString()
            .GetBytes();

        var el = new Auth
        {
            Mechanism = "PLAIN",
            Value = Convert.ToBase64String(buf)
        };

        Connection.Send(el);
    }
}
