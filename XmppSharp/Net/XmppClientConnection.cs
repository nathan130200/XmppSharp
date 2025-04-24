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

    public string User { get; set; }
    public string Server { get; set; }
    public string Resource { get; set; }
    public string Password { get; set; }
    public EndPoint ConnectServer { get; set; }
    public string AuthenticationMechanism { get; set; }
    public SslClientAuthenticationOptions SslOptions { get; set; }
    public bool UseDirectTls { get; set; }

    private SaslHandler? _saslHandler;

    protected override void Disposing()
    {
        _isConnecting = false;
        IsAuthenticated = false;
        IsSessionStarted = false;

        _saslHandler?.Dispose();
        _saslHandler = null;
    }

    volatile bool _isConnecting = false;

    SslClientAuthenticationOptions GetSslOptions()
    {
        var result = SslOptions ??= new();

        result.TargetHost ??= Server;

        result.RemoteCertificateValidationCallback ??= delegate
        {
            return true;
        };

        return result;
    }

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

            if (UseDirectTls)
            {
                var temp = new SslStream(_stream);
                await temp.AuthenticateAsClientAsync(GetSslOptions());
                _stream = temp;
            }

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

    public Func<IEnumerable<Mechanism>, Mechanism?> SaslMechanismSelector { get; set; }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (!IsAuthenticated)
        {
            if (e is StreamFeatures features)
            {
                if (features.StartTls != null && !IsTlsStarted && !UseDirectTls)
                {
                    Send(new StartTls());
                    return;
                }

                if (features.Mechanisms == null)
                    throw new JabberException("SASL not supported.");

                string mechanismName;

                if (SaslMechanismSelector == null)
                    mechanismName = !string.IsNullOrWhiteSpace(AuthenticationMechanism)
                        ? AuthenticationMechanism : "PLAIN";
                else
                {
                    var targetMechanism = SaslMechanismSelector(features.Mechanisms.SupportedMechanisms)
                                          ?? throw new JabberException("Mechanism selector didn't found any valid mechanism.");

                    mechanismName = targetMechanism.Value!;
                }

                if (!SaslFactory.TryCreate(mechanismName, this, out var handler))
                    throw new JabberException($"Unable to create SASL handler for mechanism '{mechanismName}'");

                // assign sasl handler and send first auth packet.
                _saslHandler = handler;
                _saslHandler.Init();
            }

            else if (e is Proceed)
            {
                _streamState &= ~StreamState.Read;
                _ = InitClientSsl();
                return;
            }

            else if (e is { DefaultNamespace: Namespaces.Sasl } el)
            {
                // auth success == returns true,
                // continue processing sasl = returns false

                if (_saslHandler!.Invoke(el))
                {
                    IsAuthenticated = true;
                    Jid = new(User, Server, default);
                    ResetParser();

                    _saslHandler?.Dispose();
                    _saslHandler = null!;
                }

                return;
            }
        }
        else
        {
            if (!IsSessionStarted)
            {
                if (e is StreamFeatures features)
                {
                    _ = InitSession(features.SupportsBind, features.SupportsSession);
                    return;
                }
            }
            else
            {
                if (e is Stanza stz)
                    FireOnStanza(stz);
            }
        }
    }

    void ResetParser()
    {
        _streamState &= ~StreamState.Read;

        _ = Task.Run(() =>
        {
            _parser!.Reset();
            SendStreamHeader();
            _streamState |= StreamState.Read;
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
                await ((SslStream)_stream).AuthenticateAsClientAsync(GetSslOptions());

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