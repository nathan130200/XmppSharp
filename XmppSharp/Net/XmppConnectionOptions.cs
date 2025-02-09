using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using XmppSharp.Protocol.Core.Tls;

namespace XmppSharp.Net;

public class XmppConnectionOptions
{
    public Jid Jid { get; set; }
    public EndPoint EndPoint { get; set; }
    public string Password { get; set; }
    public StartTlsPolicy SslPolicy { get; set; } = StartTlsPolicy.Required;
    public SslClientAuthenticationOptions? Ssl { get; set; }
    public ushort ReceiveBufferSize { get; set; }

    public bool EnableKeepAlive { get; set; } = true;
    public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(15);
    public TimeSpan KeepAliveTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan DisconnectTimeout { get; set; }

    public bool Verbose { get; set; }

    internal void Validate()
    {
        Ssl ??= new()
        {
            TargetHost = Jid?.Domain,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
            RemoteCertificateValidationCallback = static (a, b, c, d) => true
        };

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
