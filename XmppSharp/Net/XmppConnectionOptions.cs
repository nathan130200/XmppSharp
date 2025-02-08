using System.Net;
using System.Net.Security;
using XmppSharp.Protocol.Core.Tls;

namespace XmppSharp.Net;

public class XmppConnectionOptions
{
    public Jid Jid { get; set; }
    public EndPoint EndPoint { get; set; }
    public string Password { get; set; }
    public StartTlsPolicy SslPolicy { get; set; }
    public SslClientAuthenticationOptions? Ssl { get; set; }
    public ushort ReceiveBufferSize { get; set; }

    public bool EnableKeepAlive { get; set; }
    public TimeSpan KeepAliveInterval { get; set; }
    public TimeSpan KeepAliveTimeout { get; set; }
    public TimeSpan SendTimeout { get; set; }
    public TimeSpan ReceiveTimeout { get; set; }
    public TimeSpan DisconnectTimeout { get; set; }

    public bool OnlineVerbose { get; set; }

    internal void Validate()
    {
        if (EnableKeepAlive)
        {
            int interval = (int)KeepAliveInterval.TotalMilliseconds,
                timeout = (int)KeepAliveTimeout.TotalMilliseconds;

            if (interval <= 0 || timeout <= 0)
                throw new InvalidOperationException("Invalid keep alive settings provided.");
        }

        if (EndPoint is null)
            throw new InvalidOperationException("Endpoint is missing, unable to connect.");

        if (Jid is null)
            throw new InvalidOperationException("Jid cannot be null.");

        if (string.IsNullOrWhiteSpace(Jid.Domain))
            throw new InvalidOperationException("Domain cannot be null or empty.");

        if (SslPolicy == StartTlsPolicy.Required && Ssl == null)
            throw new InvalidOperationException("Encryption set as required however client SSL handshake parameters are missing.");
    }
}
