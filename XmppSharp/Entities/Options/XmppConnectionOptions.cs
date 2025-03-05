using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Expat;
using Microsoft.Extensions.Logging;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Entities.Options;

/// <summary>
/// Represents the base class of XMPP connection settings.
/// </summary>
public class XmppConnectionOptions
{
    /// <summary>
    /// XML encoding for the connection (in case you are using expat). Default: <see cref="ExpatEncoding.UTF8"/>
    /// </summary>
    public ExpatEncoding Encoding { get; set; } = ExpatEncoding.UTF8;

    /// <summary>
    /// Read byte allocation buffer size. Default: 4096.
    /// </summary>
    public int RecvBufferSize { get; set; } = 4096;

    /// <summary>
    /// End point for connecting to a remote server.
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    /// TLS encryption policy. Default: Required
    /// </summary>
    public TlsPolicy TlsPolicy { get; set; } = TlsPolicy.Required;

    /// <summary>
    /// TLS authentication and encryption configuration.
    /// </summary>
    public SslClientAuthenticationOptions TlsOptions { get; } = new();

    /// <summary>
    /// Starts encryption when connecting to the server (known as Direct TLS). Default: false.
    /// </summary>
    public bool StartTlsOnConnect { get; set; } = false;

    /// <summary>
    /// Authentication password.
    /// </summary>
    public string Password { get; set; } = Environment.MachineName;

    /// <summary>
    /// Instance to use for logging.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Timeout to finish the connection.
    /// <para>
    /// This time will be used to wait for the packet queue to be flushed and other pending tasks to be completed.
    /// </para>
    /// </summary>
    public TimeSpan DisconnectTimeout { get; set; } = TimeSpan.FromSeconds(2.5d);

    public XmppConnectionOptions()
    {
        // override this function to make sure handle all server certificates errors before connecting.
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