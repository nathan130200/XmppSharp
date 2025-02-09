using System.Net.Security;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;
using XmppSharp.Protocol.Core.Sasl;
using XmppSharp.Protocol.Core.Tls;

namespace XmppSharp.Net;

public class XmppClientConnectionOptions : XmppConnectionOptions
{
    public string AuthenticationMechanism { get; set; }
}

public class XmppClientConnection : XmppConnection
{
    public new XmppClientConnectionOptions Options
        => (XmppClientConnectionOptions)base.Options;

    public XmppClientConnection(XmppClientConnectionOptions options) : base(options)
    {

    }

    protected override void SendStreamHeader()
    {
        Throw.IfNull(Parser);

        InitParser();

        Send(new StreamStream
        {
            To = Options.Jid.Domain,
            Version = "1.0",
            Language = "en",
            DefaultNamespace = Namespaces.Client
        }.StartTag());
    }

    bool _isInStartTls = false,
        _isInAuth = false;

    SaslHandler? _saslHandler;

    protected override void HandleStreamElement(XmppElement e)
    {
        if (e is Stanza stz)
        {
            if (!string.IsNullOrWhiteSpace(stz.Id) && Callbacks.TryRemove(stz.Id!, out var tcs))
            {
                _ = Task.Run(() => tcs.TrySetResult(stz));
                return;
            }
        }

        if (e is StreamFeatures features)
        {
            if (!State.HasFlag(XmppConnectionState.Authenticated))
            {
                if (Stream is not SslStream && features.StartTls != null && !State.HasFlag(XmppConnectionState.Encrypted))
                {
                    if (features.StartTls.Policy == StartTlsPolicy.Required
                        || Options.SslPolicy == StartTlsPolicy.Required)
                    {
                        _isInStartTls = true;
                        Send(new StartTls());
                        return;
                    }
                }

                if (features.Mechanisms == null)
                    throw new JabberException("No auth mechanisms supported by the server.");

                var isMechanismSupported = features.Mechanisms
                    .Elements<Mechanism>()
                    .Any(x => x.Value == Options.AuthenticationMechanism);

                if (!isMechanismSupported)
                    throw new JabberSaslException(FailureCondition.InvalidMechanism);

                _isInAuth = true;
                _saslHandler = SaslHandler.CreateHandler(this, Options.AuthenticationMechanism);
                _saslHandler.Init(this);
            }
            else
            {
                _ = DoResourceBind(features);
                return;
            }
        }

        if (_isInStartTls)
        {
            if (e is Proceed)
            {
                if (!_isInStartTls || State.HasFlag(XmppConnectionState.Encrypted))
                    throw new JabberStreamException(StreamErrorCondition.UnsupportedFeature);

                _access &= ~FileAccess.Read;

                _ = Task.Run(async () =>
                {
                    Throw.IfNull(Stream);

                    var temp = new SslStream(Stream, false);
                    await temp.AuthenticateAsClientAsync(Options.Ssl!);
                    Stream = temp;
                    temp = null;

                    State |= XmppConnectionState.Encrypted;
                    _isInStartTls = false;

                    SendStreamHeader();

                    _access |= FileAccess.Read;
                });

                return;
            }

            return;
        }

        if (_isInAuth)
        {
            if (_saslHandler!.Invoke(this, e))
            {
                State |= XmppConnectionState.Authenticated;

                _isInAuth = false;

                if (_saslHandler is IDisposable disposable)
                    disposable.Dispose();

                _saslHandler = null;

                SendStreamHeader();
            }
            return;
        }

        if (e is Message message) FireOnMessage(message);
        if (e is Presence presence) FireOnPresence(presence);
        if (e is Iq iq) FireOnIq(iq);
    }

    async Task DoResourceBind(StreamFeatures features)
    {
        var bindIq = new Iq(IqType.Set);

        bindIq
            .C("bind", Namespaces.Bind)
                .C("resource", value: Options.Jid.Resource);

        var result = (Iq)await SendStanzaAsync(bindIq);

        if (result.Type == IqType.Error)
        {
            Disconnect();
            return;
        }

        State |= XmppConnectionState.ResourceBinded;

        if (features.SupportSession)
        {
            var sessionIq = new Iq(IqType.Set);

            sessionIq
                .C("session", Namespaces.Session);

            result = (Iq)await SendStanzaAsync(sessionIq);

            if (result.Type == IqType.Error)
            {
                Disconnect();
                return;
            }

            State |= XmppConnectionState.SessionStarted;
        }
    }
}