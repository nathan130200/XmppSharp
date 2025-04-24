using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Component;

namespace XmppSharp.Net;

public class XmppComponentConnection : XmppConnection
{
    public string Server { get; set; }
    public string Password { get; set; }
    public EndPoint ConnectServer { get; set; }
    public SslClientAuthenticationOptions SslOptions { get; set; }
    public bool UseDirectTls { get; set; }

    volatile bool _isConnecting = false;


    public virtual async Task ConnectAsync()
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

                temp = null;
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

    protected override void InitConnection()
    {
        Send(new StreamStream
        {
            DefaultNamespace = Namespaces.Accept,
            To = Server,
            Version = "1.0",
            Language = "en"
        });
    }

    protected override void HandleStreamStart(StreamStream e)
    {
        if (!string.IsNullOrWhiteSpace(e.Id))
        {
            StreamId = e.Id;
            Send(new Handshake(e.Id, Password));
        }
    }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (!IsAuthenticated)
        {
            if (e is Handshake && !IsAuthenticated)
            {
                IsAuthenticated = true;
                Jid = new(Server);
                FireOnSessionStarted();
            }
        }
        else
        {
            if (e is Stanza stz)
                FireOnStanza(stz);
        }
    }
}
