using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using XmppSharp.Abstractions;
using XmppSharp.Exceptions;
using Stream = XmppSharp.Protocol.Base.Stream;

namespace XmppSharp.Net.Abstractions;

public abstract class XmppOutboundConnection : XmppConnection
{
    /// <summary>
    /// Gets or sets a value indicating whether a direct TLS connection should be used.
    /// </summary>
    public bool UseDirectTLS { get; set; }
    
    /// <summary>
    /// Gets or sets the XMPP server domain name.
    /// </summary>
    public string Server { get; set; } = Environment.MachineName;
    
    /// <summary>
    /// Gets or sets the XMPP password used for authentication.
    /// </summary>
    /// <remarks>Ensure that the password is stored and transmitted securely to prevent unauthorized
    /// access.</remarks>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the network endpoint associated with the connection.
    /// </summary>
    /// <remarks>This property specifies the endpoint used for communication. It must be set to a valid 
    /// EndPoint before initiating a connection.</remarks>
    public EndPoint EndPoint { get; set; }

    /// <summary>
    /// Gets or sets a factory method for creating <see cref="Socket"/> instances.
    /// </summary>
    /// <remarks>Use this property to customize the creation of <see cref="Socket"/> instances,  such as
    /// configuring specific socket options or using a mock socket for testing.</remarks>
    public Func<Socket>? SocketFactory { get; set; }

    /// <summary>
    /// Gets or sets the SSL client authentication options used to configure the secure connection.
    /// </summary>
    /// <remarks>Use this property to customize SSL/TLS settings such as the target host, client certificates,
    /// or encryption protocols. Ensure that the options are properly configured before initiating a secure
    /// connection.</remarks>
    public SslClientAuthenticationOptions SslOptions { get; set; }

    protected SslClientAuthenticationOptions GetSslOptions()
    {
        var result = SslOptions ?? new();

        FireOnLog(XmppLogLevel.Verbose, "Init SSL options.");

        if (result.RemoteCertificateValidationCallback != null)
            FireOnLog(XmppLogLevel.Verbose, "Using provided SSL certificate validator.");
        else
        {
            FireOnLog(XmppLogLevel.Verbose, "Using default SSL certificate validator.");

            result.RemoteCertificateValidationCallback = static delegate
            {
                return true;
            };
        }

        return result;
    }

    /// <summary>
    /// Asynchronously establishes a connection to the XMPP server.
    /// </summary>
    /// <remarks>This method attempts to connect to the server specified by the <see cref="EndPoint"/>
    /// property. If a custom socket factory is provided via the <see cref="SocketFactory"/> property, it will be used
    /// to create the socket; otherwise, a default TCP socket is used. If the <see cref="UseDirectTLS"/> property is set
    /// to <see langword="true"/>, the connection will be upgraded to an encrypted SSL/TLS stream.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the connection attempt.</param>
    /// <returns></returns>
    public virtual async Task ConnectAsync(CancellationToken token = default)
    {
        if (State > XmppConnectionState.Connecting)
            return;

        GotoState(XmppConnectionState.Connecting);

        Socket? socket = default;

        try
        {
            if (SocketFactory != null)
            {
                FireOnLog(XmppLogLevel.Verbose, "Using custom TCP socket factory.");
                socket = SocketFactory();
            }
            else
            {
                FireOnLog(XmppLogLevel.Verbose, "Using default TCP socket factory.");
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }

            FireOnLog(XmppLogLevel.Verbose, $"Connecting to {EndPoint}...");
            await socket.ConnectAsync(EndPoint);

            GotoState(XmppConnectionState.Connected);

            _socket = socket;
            _stream = new NetworkStream(_socket);

            if (UseDirectTLS)
            {

                FireOnLog(XmppLogLevel.Verbose, "Using DirectTLS, upgrading to SSL socket now.");

                var temp = new SslStream(_stream);
                await temp.AuthenticateAsClientAsync(GetSslOptions(), token);
                _stream = temp;
                temp = null;

                GotoState(XmppConnectionState.Encrypted);
            }

            FireOnLog(XmppLogLevel.Verbose, "Connected successfully, setting up connection...");
            Setup(token);
        }
        catch
        {
            GotoState(XmppConnectionState.Disconnected);
            FireOnLog(XmppLogLevel.Verbose, $"Connection to server {EndPoint} failed.");
            socket?.Dispose();
            throw;
        }
    }

    protected virtual void Setup(CancellationToken token)
    {
        _disposed = 0;
        _access = FileAccess.ReadWrite;
        InitParser();

        _ = Task.Run(async () =>
        {
            try
            {
                FireOnLog(XmppLogLevel.Verbose, "Begin I/O tasks.");
                SendStreamHeader();
                await Task.WhenAll(ReadLoop(token), WriteLoop(token));
            }
            catch (Exception ex)
            {
                FireOnError(ex);
            }
            finally
            {
                FireOnLog(XmppLogLevel.Verbose, "End I/O tasks.");
                Disconnect();
            }
        }, CancellationToken.None);
    }

    //send <stream:stream: element to server

    /// <summary>
    /// Sends the initial element to the server to initiate an XML stream.
    /// </summary>
    protected virtual void SendStreamHeader()
    {

    }

    protected override void OnStreamStart(Stream e)
    {
        if (string.IsNullOrWhiteSpace(e.Id))
            throw new JabberException("The stream header 'id' attribute is missing.");
    }
}