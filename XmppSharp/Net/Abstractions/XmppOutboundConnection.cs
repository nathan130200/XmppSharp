using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Sasl;
using Stream = XmppSharp.Protocol.Base.Stream;

namespace XmppSharp.Net.Abstractions;

public abstract class XmppOutboundConnection : XmppConnection
{
    public bool UseDirectTLS { get; init; }
    public string Server { get; init; } = Environment.MachineName;
    public string Password { get; init; }
    public EndPoint EndPoint { get; init; }

    public Func<Socket>? SocketFactory { get; init; }
    public Func<IEnumerable<ISaslMechanismEntry>, ISaslMechanismEntry?> SaslMechanismSelector { get; init; }
    public LocalCertificateSelectionCallback? SslCertificateSelector { get; init; }
    public RemoteCertificateValidationCallback? SslCertificateValidator { get; init; }

    protected SslClientAuthenticationOptions BuildSslOptions()
    {
        return new SslClientAuthenticationOptions()
        {
            TargetHost = Server,
            LocalCertificateSelectionCallback = SslCertificateSelector,
            RemoteCertificateValidationCallback = SslCertificateValidator ?? delegate
            {
                return true;
            }
        };
    }

    public virtual async Task ConnectAsync(CancellationToken token = default)
    {
        if (State > XmppConnectionState.Connected)
            return;

        Socket? socket = default;

        try
        {
            GotoState(XmppConnectionState.Connected);

            if (SocketFactory != null)
                socket = SocketFactory();
            else
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            await socket.ConnectAsync(EndPoint);

            _socket = socket;
            _stream = new NetworkStream(_socket);

            if (UseDirectTLS)
            {
                var temp = new SslStream(_stream);
                await temp.AuthenticateAsClientAsync(BuildSslOptions(), token);
                _stream = temp;
                temp = null;
                GotoState(XmppConnectionState.Encrypted);
            }

            Setup(token);
        }
        catch
        {
            GotoState(XmppConnectionState.Disconnected);
            socket?.Dispose();
            throw;
        }
    }

    protected void Setup(CancellationToken token)
    {
        InitParser();
        _disposed = 0;
        _access = FileAccess.ReadWrite;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.WhenAny(ReadLoop(token), WriteLoop(token));
            }
            catch (Exception ex)
            {
                FireOnError(ex);
            }
            finally
            {
                Disconnect();
            }
        });
    }

    protected override void OnStreamStart(Stream e)
    {
        if (string.IsNullOrWhiteSpace(e.Id))
            throw new JabberException("The stream header 'id' attribute is missing.");
    }

    protected override void OnStreamElement(XmppElement e)
    {

    }
}