using Microsoft.Extensions.Logging;
using XmppSharp.Dom;
using XmppSharp.Net;
using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp.Sasl;

public abstract class XmppSaslHandler : IDisposable
{
    internal XmppClientConnection _connection;

    protected XmppClientConnection Connection => _connection;

    public XmppSaslHandler(XmppClientConnection connection)
    {
        _connection = connection;

        _connection.Logger.LogTrace("::{TypeName}()", GetType().Name);
    }

    protected virtual void Disposing()
    {

    }

    public void Dispose()
    {
        _connection.Logger.LogTrace("::~{TypeName}()", GetType().Name);

        if (_connection != null)
        {
            Disposing();
            _connection = null!;
        }

        GC.SuppressFinalize(this);
    }

    protected internal virtual void Init()
    {

    }

    protected internal virtual bool Invoke(XmppElement tag)
    {
        _connection.Logger.LogTrace("{TypeName}::Invoke({Xml})", GetType().Name, tag.ToString(false));

        if (tag is Success)
            return true;
        else if (tag is Failure failure)
            throw new JabberSaslException(failure.Condition, failure.Text);
        else
            return false;
    }
}
