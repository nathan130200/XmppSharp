using System.Runtime.CompilerServices;
using XmppSharp.Dom;
using XmppSharp.Logging;
using XmppSharp.Net.EventArgs;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Net;

/// <summary>
/// Defines the contract to manage and interact with an XMPP connection.
/// </summary>
public interface IXmppConnection
{
    /// <summary>
    /// Gets the Jabber Identifier (JID) associated with this instance.
    /// </summary>
    Jid Jid { get; }

    /// <summary>
    /// Gets a value indicating if connected to a remote server.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets the current state of the XMPP connection.
    /// </summary>
    XmppConnectionState State { get; }

    /// <summary>
    /// Gets a value indicating whether the connection is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets or sets the size, in bytes, of the receive buffer for the connection.
    /// </summary>
    /// <remarks>The receive buffer size determines the amount of data that can be buffered during data
    /// reception. Adjusting this value may impact performance depending on the application's data transfer
    /// requirements.</remarks>
    int RecvBufferSize { get; set; }

    /// <summary>
    /// Gets or sets the duration, in milliseconds, to wait while idle during a read operation.
    /// </summary>
    int ReadIdleWaitTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the duration, in milliseconds, to wait while idle before performing a write operation.
    /// </summary>
    int WriteIdleWaitTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the amount of time, in milliseconds, to wait for a disconnection to complete.
    /// </summary>
    /// <remarks>This property defines the maximum duration to wait for a gracefully disconnection.</remarks>
    int DisconnectWaitTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the logging level for logging messages.
    /// </summary>
    XmppLogScope VerbosityLevel { get; set; }

    /// <summary>
    /// Occurs when the connection state changes.
    /// </summary>
    event Action<StateChangedEventArgs>? OnStateChanged;

    /// <summary>
    /// Occurs when an XMPP element event is triggered.
    /// </summary>
    event Action<XmppElementEventArgs>? OnElement;

    /// <summary>
    /// Occurs when an log entry is generated. The <see cref="VerbosityLevel"/> property manage the verbosity level.
    /// </summary>
    event LogEventHandler? OnLog;

    /// <summary>
    /// Waits asynchronously for an XMPP element that matches the specified condition.
    /// </summary>
    /// <param name="match">A function that defines the condition to match the desired <see cref="XmppElement"/>.  The function should
    /// return <see langword="true"/> for the element that satisfies the condition.</param>
    /// <param name="name">An optional name for the callback. If not provided, a unique identifier will be generated.</param>
    /// <param name="priority">The priority of the callback, which determines the order in which callbacks are processed.  The default is <see
    /// cref="XmppCallbackPriority.Normal"/>.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="XmppElement"/>  that
    /// matches the specified condition.</returns>
    Task<XmppElement> WaitForElement(Func<XmppElement, bool> match,
        string? name = default,
        XmppCallbackPriority priority = XmppCallbackPriority.Normal,
        CancellationToken token = default,
        [CallerArgumentExpression(nameof(match))] string? expression = default);

    /// <summary>
    /// Sends a stanza of the specified type and waits asynchronously for a response.
    /// </summary>
    /// <remarks>
    /// To avoid deadlocks or callbacks that wait infinitely, always consider using a timeout cancellation token, as when the token expires it will cancel and remove the callback.
    /// </remarks>
    /// <typeparam name="TStanza">The type of the stanza to send and wait for. Must derive from <see cref="Stanza"/>.</typeparam>
    /// <param name="stz">The stanza to send. If the <see cref="Stanza.Id"/> property is null or whitespace, a new ID will be generated.</param>
    /// <param name="priority">The priority of the callback used to process the response. The default is <see cref="XmppCallbackPriority.Normal"/>.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the received stanza of type <typeparamref name="TStanza"/>.</returns>
    Task<TStanza> RequestStanzaAsync<TStanza>(TStanza stz,
        XmppCallbackPriority priority = XmppCallbackPriority.Normal,
        CancellationToken token = default)
        where TStanza : Stanza;

    /// <summary>
    /// Sends the specified XMPP element.
    /// </summary>
    void Send(XmppElement e);

    /// <summary>
    /// Sends the specified XMPP element asynchronously.
    /// </summary
    Task SendAsync(XmppElement e);

    /// <summary>
    /// Disconnects the current XMPP connection, optionally sending a final XMPP element before closing.
    /// </summary>
    /// <param name="element">An optional <see cref="XmppElement"/> to send as the final XML before disconnecting. 
    /// If <see langword="null"/>, no additional message is sent.</param>
    void Disconnect(XmppElement? element = null);

    /// <summary>
    /// Disconnects the current session with the specified error condition and optional descriptive text.
    /// </summary>
    /// <param name="condition">The error condition that describes the reason for the disconnection.</param>
    /// <param name="text">An optional descriptive text providing additional context for the disconnection. Can be <see langword="null"/>.</param>
    void Disconnect(StreamErrorCondition condition, string? text = default);
}
