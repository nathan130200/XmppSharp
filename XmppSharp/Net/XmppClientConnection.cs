using System.Globalization;
using System.Net.Security;
using Microsoft.Extensions.Logging;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;
using XmppSharp.Protocol.Core.Client;
using XmppSharp.Protocol.Core.Sasl;
using XmppSharp.Protocol.Core.Tls;
using XmppSharp.Sasl;

namespace XmppSharp.Net;

public class XmppClientConnection : XmppConnection
{
    public new XmppClientConnectionOptions Options
        => (base.Options as XmppClientConnectionOptions)!;

    public XmppClientConnection(XmppClientConnectionOptions options) : base(options)
    {

    }

    volatile byte _phase;

    protected override void SendStreamHeader()
    {
        var xml = new StreamStream
        {
            To = Options.Domain,
            DefaultNamespace = Namespaces.Client,
            Language = CultureInfo.CurrentCulture.Name,
            Version = "1.0",
        };

        Logger.LogDebug("Sending stream start to {Hostname}", Options.Domain);

        Send(xml.StartTag());
    }

    XmppSaslHandler? _saslHandler;

    protected override void HandleStreamStart(StreamStream e)
    {
        StreamId = e.Id;

        Logger.LogDebug("Stream start received from remote server. StreamID: {Id}", StreamId);
    }

    const int PHASE_INIT = 0;
    const int PHASE_TLS = 1;
    const int PHASE_AUTH = 2;
    const int PHASE_READY = 4;

    protected override void Disposing()
    {
        base.Disposing();

        _phase = PHASE_INIT;
        _saslHandler?.Dispose();
        _saslHandler = null;
    }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (_phase == PHASE_INIT)
        {
            if (e is StreamFeatures features)
            {
                if (!IsAuthenticated)
                {
                    Logger.LogDebug("Server features received. Client is not authenticated");

                    if (!IsEncrypted)
                    {
                        var isServerSupported = false;
                        var isServerRequired = false;
                        var isClientRequired = Options.TlsPolicy == TlsPolicy.Required;

                        if (features.StartTls != null)
                        {
                            isServerSupported = true;
                            isServerRequired = features.StartTls.Policy == TlsPolicy.Required;
                            Logger.LogDebug("The server offers TLS encryption ({Type}).", isServerRequired ? "required" : "optional");
                        }

                        if (isClientRequired && !isServerSupported)
                        {
                            Logger.LogDebug("The client needs the secure channel, but the server does not offer it.");
                            Disconnect();
                            return;
                        }

                        if (!isClientRequired && isServerRequired)
                        {
                            Logger.LogDebug("The server offers but the client does not want to establish a secure channel.");
                            Disconnect();
                            return;
                        }

                        if (isServerSupported)
                        {
                            Logger.LogDebug("Sending start TLS");
                            _phase = PHASE_TLS;
                            Send(new StartTls());
                            return;
                        }
                    }

                    if (features.Mechanisms == null)
                    {
                        Logger.LogDebug("Server did not offer any authentication mechanism?");
                        throw new JabberStreamException(StreamErrorCondition.UnsupportedFeature, "The server did not offer any authentication mechanism.");
                    }

                    var mechanismName = Options.AuthenticationMechanism;

                    if (string.IsNullOrWhiteSpace(Options.AuthenticationMechanism))
                    {
                        mechanismName = features.Mechanisms.SupportedMechanisms.FirstOrDefault()?.Value;
                        Logger.LogDebug("Using server provided authentication mechanism: {Name}", mechanismName);
                    }
                    else
                    {
                        Logger.LogDebug("Trying to use client provided authentication mechanism: {Name}", Options.AuthenticationMechanism);

                        var isSupported = features.Mechanisms.SupportedMechanisms.Any(x => x.Value == Options.AuthenticationMechanism);

                        if (!isSupported)
                        {
                            Logger.LogDebug("Server does not support client provided authentication mechanism: {Name}", mechanismName);
                            throw new JabberSaslException(FailureCondition.InvalidMechanism, "Unable to authenticate with a known mechanism.");
                        }
                    }

                    _phase = PHASE_AUTH;

                    if (!XmppSaslMechanismFactory.TryCreate(mechanismName!, this, out _saslHandler))
                    {
                        Logger.LogDebug("The requested authentication mechanism '{Name}' was selected, but is not implemented.", mechanismName);
                        throw new JabberSaslException(FailureCondition.InvalidMechanism, $"Mechanism '{mechanismName}' is not registered in SASL factory.");
                    }

                    Logger.LogDebug("Begin SASL.");

                    _saslHandler.Init();
                }
                else
                {
                    Logger.LogDebug("Server features received. Client is authenticated. Init session");
                    InitSession(features.SupportBind, features.SupportSession);
                }
            }
        }
        else if (_phase == PHASE_TLS)
        {
            Logger.LogDebug("Begin TLS");

            if (e is not Proceed)
            {
                Logger.LogDebug("Unexpected XML element during start TLS");
                throw new JabberStreamException(StreamErrorCondition.InvalidXml);
            }

            Logger.LogDebug("Pause IO read");
            _access &= ~FileAccess.Read;

            QueueTask(async token =>
            {
                Logger.LogDebug("new SSLStream");
                var temp = new SslStream(_stream!);

                await temp.AuthenticateAsClientAsync(Options.TlsOptions, token);
                Logger.LogDebug("Encrypt TLS as client");

                _stream = temp;
                temp = null;

                Logger.LogDebug("Reset parser");
                _parser!.Reset();

                ChangeState(x => x | XmppConnectionState.Encrypted);

                SendStreamHeader();

                Logger.LogDebug("Resume IO read");
                _access |= FileAccess.Read;

                Logger.LogDebug("Restart xmpp cycle");
                _phase = PHASE_INIT;
            });
        }
        else if (_phase == PHASE_AUTH)
        {
            // true = auth success
            // false = continue processing auth elements
            // auth failed = JabberSaslException is thrown

            var result = _saslHandler!.Invoke(e);

            Logger.LogDebug("SASL Handler result: {State}", result);

            if (!result)
                Logger.LogDebug("SASL handler update: not authenticated yet (continue)");
            else
            {
                Logger.LogDebug("SASL handler update: authentication success (finish)");

                Jid = new Jid(Options.Username, Options.Domain, default);

                _saslHandler.Dispose();
                _saslHandler = null;

                Logger.LogDebug($"Pause IO read");
                _access &= ~FileAccess.Read;

                QueueTask(async _ =>
                {
                    await Task.Yield();

                    ChangeState(x => x | XmppConnectionState.Authenticated);

                    Logger.LogDebug($"Restart xmpp cycle");
                    _phase = PHASE_INIT;
                    _parser!.Reset();
                    SendStreamHeader();

                    Logger.LogDebug($"Resume IO read");
                    _access |= FileAccess.Read;
                });
            }
        }
        else if (_phase == PHASE_READY)
        {
            FireOnElement(e);
        }
    }

    void InitSession(bool supportBind, bool supportSession)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (supportBind)
                {
                    Logger.LogDebug($"Post-Features: Server support resource binding...");
                    await DoResourceBind();
                }

                if (supportSession)
                {
                    Logger.LogDebug($"Post-Features: Server support session init...");
                    await DoSessionStart();
                }

                if (Options.InitialPresence != null)
                {
                    Logger.LogDebug("Sending initial presence set.");
                    var el = new Presence(Options.InitialPresence);
                    el.GenerateId();
                    _ = RequestStanzaAsync(el);
                }

                FireOnOnline();

                Logger.LogDebug("{Jid} Client is online.", Jid);

                _phase = PHASE_READY;
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, "An error occurred during session initialization.");
                Disconnect();
            }
        });
    }

    async Task DoResourceBind()
    {
        var request = new Iq(IqType.Set);

        request.AddChild(new Bind(Options.Resource));

        if (string.IsNullOrWhiteSpace(Options.Resource))
            Logger.LogDebug("Resource bind was requested with empty resource");

        var response = await RequestStanzaAsync(request);

        if (response.Type == IqType.Error)
            throw new JabberException("Resource bind failed - " + (response.Error?.Condition ?? StanzaErrorCondition.UndefinedCondition));

        var serverResource = response.Element<Bind>()?.Jid?.Resource;

        if (string.IsNullOrWhiteSpace(serverResource))
            throw new JabberStreamException(StreamErrorCondition.InvalidXml, "Server did not send a valid JID.");

        Logger.LogDebug("Server {Action} client resource to: {Value}",
            serverResource == Options.Resource ? "authorized" : "replaced", serverResource);

        Jid = Jid with { Resource = serverResource };

        ChangeState(x => x | XmppConnectionState.ResourceBinded);
    }

    async Task DoSessionStart()
    {
        var request = new Iq(IqType.Set);

        request.AddChild(new Session());

        var response = await RequestStanzaAsync(request);

        if (response.Type == IqType.Error)
            throw new JabberException("Session start failed - " + (response.Error?.Condition ?? StanzaErrorCondition.UndefinedCondition));

        ChangeState(x => x | XmppConnectionState.SessionStarted);
    }
}