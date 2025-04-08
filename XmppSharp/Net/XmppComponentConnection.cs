using System.Net;
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
