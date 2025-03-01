using XmppSharp.Dom;
using XmppSharp.Net;
using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp.Sasl;

public abstract class XmppSaslMechanismHandler
{
    protected XmppClientConnection Connection { get; }

    public XmppSaslMechanismHandler(XmppClientConnection connection)
        => Connection = connection;

    protected internal virtual void Init()
    {

    }

    protected internal virtual bool Invoke(XmppElement tag)
    {
        if (tag is Success)
            return true;
        else if (tag is Failure failure)
            throw new JabberSaslException(failure.Condition, failure.Text);
        else
            return false;
    }
}
