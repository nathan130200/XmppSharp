using System.Globalization;
using System.Net.Security;
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
    volatile byte _phase;

    protected override void SendStreamHeader()
    {
        var xml = new StreamStream
        {
            To = Options.Jid.Domain,
            DefaultNamespace = Namespaces.Client,
            Language = CultureInfo.CurrentCulture.Name,
            Version = "1.0",
        };

        Send(xml.StartTag());
    }

    XmppSaslMechanismHandler? _authHandler;

    protected override void HandleStreamStart(StreamStream e)
    {

    }

    const int PHASE_INIT = 0;
    const int PHASE_TLS = 1;
    const int PHASE_AUTH = 2;
    const int PHASE_READY = 4;

    protected override void Disposing()
    {
        _phase = PHASE_INIT;

        if (_authHandler != null)
        {
            _authHandler._connection = null!;
            _authHandler = null;
        }
    }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (e is StreamError se)
            throw new JabberStreamException(se.Condition ?? StreamErrorCondition.InternalServerError, se.Text);

        if (_phase == PHASE_INIT)
        {
            if (e is StreamFeatures features)
            {
                if (!IsAuthenticated)
                {
                    if (!IsEncrypted)
                    {
                        bool isServerSupported = false;
                        bool isClientRequired = Options.TlsPolicy == StartTlsPolicy.Required;

                        if (features.StartTls != null)
                            isServerSupported = true;

                        if (isClientRequired && !isServerSupported)
                            throw new JabberStreamException(StreamErrorCondition.UnsupportedFeature, "The client requires TLS but the server did not offer it.");

                        if (isServerSupported)
                        {
                            _phase = PHASE_TLS;
                            Send(new StartTls());
                            return;
                        }
                    }

                    if (features.Mechanisms == null)
                        throw new JabberStreamException(StreamErrorCondition.UnsupportedFeature, "The server did not offer any authentication mechanism.");

                    var mechanismName = Options.AuthenticationMechanism;
                    var useKnownMechanism = false;

                    if (string.IsNullOrWhiteSpace(Options.AuthenticationMechanism))
                    {
                        mechanismName = features.Mechanisms.SupportedMechanisms.FirstOrDefault()?.Value;
                        useKnownMechanism = true;
                    }

                    if (!useKnownMechanism)
                    {
                        var isSupported = features.Mechanisms.SupportedMechanisms.Any(x => x.Value == Options.AuthenticationMechanism);

                        if (!isSupported)
                            throw new JabberSaslException(FailureCondition.InvalidMechanism, "Unable to authenticate with a known mechanism.");
                    }

                    _phase = PHASE_AUTH;
                    _authHandler = XmppSaslMechanismFactory.CreateNew(this, mechanismName!);
                    _authHandler.Init();
                }
                else
                {
                    InitSession(features.SupportBind, features.SupportSession);
                }
            }
        }
        else if (_phase == PHASE_TLS)
        {
            if (e is not Proceed)
                throw new JabberStreamException(StreamErrorCondition.UnsupportedStanzaType);

            _access &= ~FileAccess.Read;

            QueueTask(async token =>
            {
                var temp = new SslStream(_stream!);
                await temp.AuthenticateAsClientAsync(Options.TlsOptions, token);
                _stream = temp;
                temp = null;

                _parser!.Reset();
                ChangeState(x => x | XmppConnectionState.Encrypted);
                SendStreamHeader();
                _access |= FileAccess.Read;
                _phase = PHASE_INIT;
            });
        }
        else if (_phase == PHASE_AUTH)
        {
            if (_authHandler!.Invoke(e))
            {
                _authHandler._connection = null!;
                _authHandler = null;
                _access &= ~FileAccess.Read;

                QueueTask(async _ =>
                {
                    await Task.Yield();

                    ChangeState(x => x | XmppConnectionState.Authenticated);
                    _phase = PHASE_INIT;
                    _parser!.Reset();
                    SendStreamHeader();
                    _access |= FileAccess.Read;
                });
            }
        }
        else if (_phase == PHASE_READY)
        {
            if (e is Stanza stz)
                _ = Task.Run(() => FireOnStanza(stz));
        }
    }

    void InitSession(bool supportBind, bool supportSession)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (supportBind)
                    await DoResourceBind();

                if (supportSession)
                    await DoSessionStart();

                _phase = PHASE_READY;
            }
            catch (Exception ex)
            {
                FireOnError(ex);
                Disconnect();
            }
        });
    }

    async Task DoResourceBind()
    {
        var request = new Iq(IqType.Set);
        request.AddChild(new Bind(Options.Jid.Resource ?? Environment.MachineName));

        var response = await RequestStanzaAsync(request) ?? throw new JabberException("Unexpected response.");

        if (response.Type == IqType.Error)
            throw new JabberException("Resource bind failed: " + (response.Error?.Condition ?? StanzaErrorCondition.UndefinedCondition));

        ChangeState(x => x | XmppConnectionState.ResourceBinded);
    }

    async Task DoSessionStart()
    {
        var request = new Iq(IqType.Set);
        request.AddChild(new Session());

        var response = await RequestStanzaAsync(request) ?? throw new JabberException("Unexpected response.");

        if (response.Type == IqType.Error)
            throw new JabberException("Session start failed: " + (response.Error?.Condition ?? StanzaErrorCondition.UndefinedCondition));

        ChangeState(x => x | XmppConnectionState.SessionStarted);
    }
}