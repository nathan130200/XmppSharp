using System.Net.Security;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Logging;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;
using XmppSharp.Sasl;

namespace XmppSharp.Net;

/// <summary>
/// Represents an outbound XMPP client-to-server (C2S) connection.
/// </summary>
/// <remarks>This class provides functionality for establishing and managing an outbound XMPP connection,
/// including authentication, encryption, and session initialization.
/// </remarks>
public class OutgoingXmppClientConnection : OutgoingXmppConnection
{
    /// <summary>
    /// Gets or sets the client username used in SASL authentication.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the XMPP resource associated with the entity.
    /// </summary>
    /// <remarks>The resource is used in XMPP to distinguish between multiple connections or sessions
    /// associated with the same bare JID (Jabber ID). It plays a  critical role in routing messages to the correct
    /// session when multiple resources are connected under the same bare JID.</remarks>
    public string Resource { get; set; }

    /// <summary>
    /// Presence priority for this XMPP connection and its bound resource.
    /// </summary>
    /// <remarks>
    /// Affects how the server routes stanzas addressed to the bare JID (user@domain).
    /// Values less than zero indicate that this resource should not receive stanzas sent to the bare JID.
    /// Higher values typically get delivery preference, but actual routing may depend on server-specific logic.
    /// </remarks>
    public sbyte Priority { get; set; } = 1;

    /// <summary>
    /// Gets or sets a delegate that selects the SASL authentication mechanism to use from the provided list of server-supported mechanisms.
    /// </summary>
    /// <remarks>
    /// If this property is set to <see langword="null"/>, the client will reject all server-provided
    /// authentication mechanisms, and the connection will be closed. If this property is not explicitly set,
    /// the first available mechanism from the server's list will be used by default.
    /// </remarks>
    public Func<IEnumerable<ISaslMechanism>, ISaslMechanism?>? MechanismSelector { get; init; }

    /// <summary>
    /// Gets or sets the encryption policy for TLS connections.
    /// </summary>
    /// <remarks>Use this property to specify the desired level of encryption for TLS connections.  Adjusting
    /// this policy may affect the security and compatibility of the connection.</remarks>
    public TlsPolicy EncryptionPolicy { get; set; } = TlsPolicy.Required;

    /// <summary>
    /// Gets or sets the timeout, in milliseconds, for completing a TLS handshake.
    /// </summary>
    /// <remarks>
    /// Use this property to configure the maximum allowable time for establishing a secure connection.
    /// </remarks>
    public int TlsHandshakeTimeoutMs { get; set; } = 5000;

    private XmppSaslHandler? _saslHandler;

    protected override void InitConnection()
    {
        Send(new Protocol.Base.Stream
        {
            DefaultNamespace = Namespaces.Client,
            To = Server,
            Version = "1.0",
            Language = "en"
        });
    }

    protected virtual void ResetStream()
    {
        _access &= ~FileAccess.Read;

        _ = Task.Run(() =>
        {
            _parser!.Reset();
            _access |= FileAccess.Read;
            InitConnection();
        });
    }

    protected virtual void InitSsl()
    {
        _access &= ~FileAccess.Read;

        _ = Task.Run(async () =>
        {
            try
            {
                FireOnLog(XmppLogScope.Encryption, "Starting TLS handshake...");

                using var cts = new CancellationTokenSource(TlsHandshakeTimeoutMs);
                var temp = new SslStream(_stream!);
                await temp.AuthenticateAsClientAsync(GetSslOptions(), cts.Token);
                _stream = temp;
                temp = null;

                GotoState(XmppConnectionState.Encrypted);
                ResetStream();
            }
            catch (OperationCanceledException ex)
            {
                FireOnLog(XmppLogScope.Encryption, "TLS handshake timeout.", ex);
                Dispose();
            }
            catch (Exception ex)
            {
                FireOnLog(XmppLogScope.Encryption, "TLS handshake failed.", ex);
                Dispose();
            }
        });
    }

    protected override void OnStreamElement(XmppElement e)
    {
        ProcessCallbacks(e);

        if (State < XmppConnectionState.Authenticated)
        {
            if (e is StreamFeatures features)
            {
                if (State < XmppConnectionState.Encrypted)
                {
                    if (EncryptionPolicy == TlsPolicy.Required)
                    {
                        if (features.StartTls?.Policy != TlsPolicy.Required)
                            throw new JabberStreamException(StreamErrorCondition.UnsupportedFeature, "The server does not provide encryption but the client requires it.");

                        goto next;
                    }
                    else if (EncryptionPolicy == TlsPolicy.None && features.StartTls?.Policy != TlsPolicy.None)
                        throw new JabberStreamException(StreamErrorCondition.PolicyViolation, "The server offers encryption but the connection does not accept encryption.");

                    next:
                    {
                        Send(new StartTls());
                        return;
                    }
                }

                if (features.Mechanisms == null)
                    throw new JabberStreamException(StreamErrorCondition.UnsupportedFeature, "The remote server does not support SASL authentication.");

                var mechanisms = features.Mechanisms.Nodes()
                    .OfType<ISaslMechanism>();

                if (MechanismSelector != null)
                {
                    var mechanism = MechanismSelector(mechanisms);

                    if (mechanism == null)
                        goto _no_mechanism;

                    if (XmppSaslHandlerFactory.TryCreate(mechanism.MechanismName!, this, out _saslHandler))
                        goto _init_sasl;
                }
                else
                {
                    foreach (var mechanism in mechanisms)
                    {
                        if (XmppSaslHandlerFactory.TryCreate(mechanism.MechanismName!, this, out _saslHandler))
                            goto _init_sasl;
                    }
                }

            _no_mechanism:
                {
                    if (_saslHandler == null)
                        throw new JabberSaslException(FailureCondition.Aborted, "No suitable authentication mechanism found.");
                }

            _init_sasl:
                {
                    FireOnLog(XmppLogScope.Connection, $"Using SASL mechanism implementation '{_saslHandler.GetType().FullName}'.");
                    _saslHandler.Init();
                    return;
                }
            }
            else if (e is Proceed)
            {
                InitSsl();
                return;
            }

            var result = _saslHandler!.Invoke(e);

            if (result.Type != XmppSaslHandlerResultType.Continue)
            {
                if (result.Type == XmppSaslHandlerResultType.Success)
                {
                    Jid = new(User, Server);
                    GotoState(XmppConnectionState.Authenticated);
                    ResetStream();
                }
                else if (result.Type == XmppSaslHandlerResultType.Error)
                {
                    FireOnLog(XmppLogScope.Connection, "Authentication failed.", result.Exception);
                    Disconnect();
                }

                _saslHandler?.Dispose();
                _saslHandler = null;
                return;
            }
        }
        else
        {
            if (State < XmppConnectionState.SessionStarted)
            {
                if (e is StreamFeatures features)
                {
                    _ = InitSession(features.SupportsBind, features.SupportsSession);
                    return;
                }

                FireOnElement(e);
            }
            else
            {
                FireOnElement(e);
            }
        }
    }

    async Task InitSession(bool supportsBind, bool supportsSession)
    {
        try
        {
            if (supportsBind)
                await DoResourceBind();

            if (supportsSession)
                await DoSessionStart();

            GotoState(XmppConnectionState.SessionStarted);
        }
        catch (Exception ex)
        {
            FireOnLog(XmppLogScope.Connection, exception: ex);
            Disconnect();
        }
    }

    async Task DoResourceBind()
    {
        var result = await RequestStanzaAsync(new Iq()
        {
            Type = IqType.Set,
            Query = new Bind { Resource = Resource }
        });

        if (result.Type == IqType.Error)
        {
            string? msg = null;

            if (result.Error != null)
                msg = $" (reason='{result.Error.Text}'; code={result.Error.Condition})";

            throw new JabberException($"Resource bind failed.{msg}");
        }

        var resource = (result.Query as Bind)?.Jid?.Resource ?? Resource;

        Jid = new(Jid)
        {
            Resource = resource
        };

        FireOnLog(XmppLogScope.Connection, $"Using client JID {Jid} as current.");
    }

    async Task DoSessionStart()
    {
        var result = await RequestStanzaAsync(new Iq()
        {
            Type = IqType.Set,
            Query = new Session()
        });

        if (result.Type == IqType.Error)
        {
            string? msg = null;

            if (result.Error != null)
                msg = $" (reason='{result.Error.Text}'; code={result.Error.Condition})";

            throw new JabberException($"Session start failed.{msg}");
        }

        FireOnLog(XmppLogScope.Connection, "Client session started.");
    }
}
