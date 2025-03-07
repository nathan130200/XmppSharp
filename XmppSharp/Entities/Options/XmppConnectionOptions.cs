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
    /// XML char encoding. Default: <see cref="ExpatEncoding.UTF8"/>
    /// </summary>
    public ExpatEncoding Encoding { get; set; } = ExpatEncoding.UTF8;

    /// <summary>
    /// Determines whether the xmpp connection will treat unknown elements 
    /// (i.e. not part of the rules defined in RFC-6120) as a violation of the XMPP protocol and immediately disconnect from the server.
    /// Default: <see langword="true" />
    /// </summary>
    public bool TreatUnknownElementAsProtocolViolation { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the keep-alive mechanism is enabled.
    /// </summary>
    public bool EnableKeepAlive { get; set; } = true;

    /// <summary>
    /// Gets or sets the interval at which keep-alive signals are sent.
    /// </summary>
    /// <remarks>
    /// The default value is 15 seconds.
    /// </remarks>
    public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the timeout duration before a keep-alive failure is detected.
    /// </summary>
    /// <remarks>
    /// The default value is 5 seconds.
    /// </remarks>
    public TimeSpan KeepAliveTimeout { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Input buffer size. Default: <c>4096</c>.
    /// </summary>
    public int RecvBufferSize { get; set; } = 4096;

    /// <summary>
    /// Endpoint for connecting to a remote server. <b>(required)</b>
    /// </summary>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    /// TLS encryption policy. Default: <see cref="TlsPolicy.Required"/>
    /// </summary>
    public TlsPolicy TlsPolicy { get; set; } = TlsPolicy.Required;

    /// <summary>
    /// TLS authentication and encryption configuration.
    /// </summary>
    public SslClientAuthenticationOptions TlsOptions { get; } = new();

    /// <summary>
    /// Starts encryption when connecting to the server (known as Direct TLS). Default: <see langword="false" />.
    /// </summary>
    public bool StartTlsOnConnect { get; set; } = false;

    /// <summary>
    /// Authentication password. <b>(required)</b>
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

    protected internal virtual void Validate()
    {
        if (EndPoint is null)
            throw new InvalidOperationException("Cannot connect: EndPoint is not set.");

        if (Password is null)
            throw new InvalidOperationException("Password must be set.");

        if (TlsPolicy > TlsPolicy.Unknown && TlsOptions == null)
            throw new InvalidOperationException("TLS options cannot be null");


        TlsOptions.RemoteCertificateValidationCallback ??= DefaultRemoteServerCertificateValidator;
    }

    protected internal virtual string DefaultNamespace { get; }

    static bool DefaultRemoteServerCertificateValidator(object sender,
        X509Certificate? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}