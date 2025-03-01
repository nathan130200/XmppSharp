using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Expat;
using XmppSharp.Protocol.Core.Tls;

namespace XmppSharp.Net;

public class XmppConnectionOptions
{
    public ExpatEncoding Encoding { get; set; } = ExpatEncoding.UTF8;
    public int RecvBufferSize { get; set; } = 4096;
    public EndPoint EndPoint { get; set; }
    public StartTlsPolicy? TlsPolicy { get; set; } = StartTlsPolicy.Optional;
    public SslClientAuthenticationOptions TlsOptions { get; } = new();
    public bool StartTlsOnConnect { get; set; } = false;
    public Jid Jid { get; set; }
    public string Password { get; set; } = Environment.MachineName;
    public bool Verbose { get; set; }
    public TimeSpan DisconnectTimeout { get; set; } = TimeSpan.FromSeconds(2.5d);
    public string? AuthenticationMechanism { get; set; }

    public XmppConnectionOptions()
    {
        TlsOptions.RemoteCertificateValidationCallback = DefaultRemoteServerCertificateValidator;
    }

    static bool DefaultRemoteServerCertificateValidator(object sender,
        X509Certificate? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}