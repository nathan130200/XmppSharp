using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;
using XmppSharp.Sasl;

namespace XmppSharp.Net;

public sealed class XmppClientConnection : XmppConnection
{
    public bool IsTlsStarted => _stream is SslStream;
    public bool IsSessionStarted { get; private set; }

    public string User { get; set; }
    public string Server { get; set; }
    public string Resource { get; set; }
    public string Password { get; set; }
    public EndPoint ConnectServer { get; set; }
    public string AuthenticationMechanism { get; set; }
    public SslClientAuthenticationOptions SslOptions { get; set; } = new();

    private SaslHandler _saslHandler;

    protected override void Cleanup()
    {
        _isConnecting = false;
        IsAuthenticated = false;
        IsSessionStarted = false;
    }

    volatile bool _isConnecting = false;

    public async Task ConnectAsync()
    {
        if (_isConnecting)
            return;

        _isConnecting = true;

        try
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(ConnectServer);
            _stream = new NetworkStream(socket, true);
            FireOnConnected();
            InitParser();
        }
        catch
        {
            Dispose();
            throw;
        }
        finally
        {
            _isConnecting = false;
        }
    }

    protected override void InitConnection()
    {
        SendStreamHeader();
    }

    void SendStreamHeader()
    {
        Send(new StreamStream
        {
            To = Server,
            DefaultNamespace = Namespaces.Client,
            Version = "1.0",
            Language = "en"
        });
    }

    protected override void HandleStreamStart(StreamStream e)
    {
        StreamId = e.Id!;
    }

    public Func<IEnumerable<Mechanism>, Mechanism> AuthenticationMechanismSelector { get; set; }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (!IsAuthenticated)
        {
            if (e is StreamFeatures features)
            {
                if (features.StartTls != null && _stream is not SslStream)
                {
                    Send(new StartTls());
                    return;
                }

                if (features.Mechanisms == null)
                    throw new JabberException("SASL not supported.");

                var mechanismName = !string.IsNullOrWhiteSpace(AuthenticationMechanism)
                    ? AuthenticationMechanism : "PLAIN";

                var useSelector = true;

            _validateMechanism:

                if (!features.Mechanisms.IsMechanismSupported(mechanismName!))
                {
                    if (useSelector && AuthenticationMechanismSelector != null)
                    {
                        mechanismName = AuthenticationMechanismSelector(features.Mechanisms!.SupportedMechanisms)?.Value;

                        if (string.IsNullOrWhiteSpace(mechanismName))
                        {
                            // client rejected all mechanisms
                            goto _invalidMechanism;
                        }

                        // attempt to validate mechanism again based on server provided mechanisms.
                        useSelector = false;
                        goto _validateMechanism;
                    }

                _invalidMechanism:

                    // we cannot authenticate
                    throw new JabberException($"SASL mechanism '{mechanismName}' not supported.");
                }

                // attempt to create sasl handler from mechanism name.
                if (!SaslFactory.TryCreate(mechanismName, this, out var handler))
                {
                    // mechanism not implemented by the client or extending library.
                    throw new JabberException($"Unable to create SASL handler for mechanism '{mechanismName}'");
                }

                // assign sasl handler and send first auth packet.
                _saslHandler = handler;
                _saslHandler.Init();
            }

            else if (e is Proceed)
            {
                _ioState &= ~IoState.Read;
                _ = InitClientSsl();
                return;
            }

            else if (e is { DefaultNamespace: Namespaces.Sasl } el)
            {
                if (_saslHandler.Invoke(el)) // auth success == returns true,
                                             // continue processing sasl = returns false
                {
                    IsAuthenticated = true;
                    Jid = new(User, Server, default);
                    ResetParser();

                    _saslHandler?.Dispose();
                    _saslHandler = null!;
                }

                return;
            }

            else
            {
                throw new JabberException("Unsupported stanza type.");
            }
        }
        else
        {
            if (!IsSessionStarted)
            {
                if (e is StreamFeatures features)
                    _ = InitSession(features.SupportsBind, features.SupportsSession);
            }
            else
            {
                if (e is Stanza stz)
                {
                    if (stz is Iq iq && iq.Query is { TagName: "ping", Namespace: Namespaces.Ping })
                    {
                        iq.SwitchDirection();
                        iq.Type = IqType.Result;
                        Send(iq);
                        return;
                    }

                    FireOnStanza(stz);
                }
            }
        }
    }

    void ResetParser()
    {
        _ioState &= ~IoState.Read;

        _ = Task.Run(() =>
        {
            _parser!.Reset();
            SendStreamHeader();
            _ioState |= IoState.Read;
        });
    }

    async Task InitClientSsl()
    {
        try
        {
            var tcs = new TaskCompletionSource();
            AddToSendQueue(new() { Completion = tcs });
            await Task.WhenAny(tcs.Task, Task.Delay(1500));

            try
            {
                _stream = new SslStream(_stream!);
                SslOptions.TargetHost ??= Server;
                await ((SslStream)_stream).AuthenticateAsClientAsync(SslOptions);

                ResetParser();
            }
            finally
            {
                tcs.TrySetResult();
            }
        }
        catch (Exception ex)
        {
            FireOnError(ex);
            Dispose();
        }
    }

    async Task InitSession(bool doResourceBind, bool doSession)
    {
        try
        {
            if (doResourceBind)
                await DoResourceBind();

            // compatibility layer for older servers.
            // session establishment feature is not mandatory
            if (doSession)
                await DoSessionStart();

            IsSessionStarted = true;

            FireOnSessionStarted();
        }
        catch (Exception ex)
        {
            FireOnError(ex);
            Disconnect();
        }
    }

    async Task DoResourceBind()
    {
        var result = (Iq)await RequestStanza(new Iq
        {
            Type = IqType.Set,
            Query = new Bind { Resource = Resource }
        });

        if (result.Type == IqType.Error)
            throw new JabberException($"Resource bind failed. (code='{result.Error?.Condition}'; reason='{result.Error?.Text}')");

        // Use our resource or use server provided assigned resource.
        var resource = (result.Query as Bind)?.Jid?.Resource ?? Resource;

        Jid = new(Jid)
        {
            Resource = resource
        };
    }

    async Task DoSessionStart()
    {
        var result = (Iq)await RequestStanza(new Iq
        {
            Type = IqType.Set,
            Query = new Session()
        });

        if (result.Type == IqType.Error)
            throw new JabberException($"Session start failed. (code='{result.Error?.Condition}'; reason='{result.Error?.Text}')");
    }
}