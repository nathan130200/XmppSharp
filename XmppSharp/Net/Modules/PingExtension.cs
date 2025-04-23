using System.Diagnostics;
using System.Security.Cryptography;
using XmppSharp.Net.Extensions.Abstractions;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Extensions.XEP0199;

namespace XmppSharp.Net.Extensions;

public class PingExtensionOptions
{
    /// <summary>
    /// Determines the ping interval between requests.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Determines the timeout until the ping is considered lost (server did not respond).
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Determines whether the client will be allowed to send ping requests to the server.
    /// <para>
    /// Default <see langword="true" />. If set to <see langword="false" /> only ping requests from the server will be processed.
    /// </para>
    /// </summary>
    public bool UseClientPingRequests { get; set; } = true;

    public static PingExtensionOptions Default => new()
    {
        UseClientPingRequests = false
    };
}

public class PingExtension : BaseExtension
{
    private readonly PingExtensionOptions _options;
    private TaskCompletionSource<bool> _tcs;
    private DateTimeOffset _startTime;
    private string _stanzaId;

    public event Action<int> OnElapsed;

    public PingExtension() : this(PingExtensionOptions.Default)
    {

    }

    public PingExtension(PingExtensionOptions options)
    {
        _options = options;

        if (_options.UseClientPingRequests)
        {
            if (_options.Interval <= TimeSpan.Zero)
                throw new InvalidOperationException("Ping interval must be greater than or equal to zero.");

            if (_options.Timeout <= TimeSpan.Zero)
                throw new InvalidOperationException("Ping timeout must be greater than or equal to zero.");
        }
    }

    protected internal override void Setup()
    {
        Connection.OnStanza += OnStanza;

        if (_options.UseClientPingRequests)
        {
            _ = Task.Run(async () =>
            {
                while (Connection != null)
                {
                    try
                    {
                        await Connection.SendAsync(new Iq
                        {
                            Type = IqType.Get,
                            Id = _stanzaId = RandomNumberGenerator.GetHexString(16, true),
                            Query = new Ping()
                        });

                        _tcs = new TaskCompletionSource<bool>();

                        var timeout = Task.Delay(_options.Timeout);

                        _startTime = DateTime.UtcNow;

                        if (await Task.WhenAny(_tcs.Task, timeout) == timeout)
                            Connection.Disconnect();
                        else
                        {
                            var rtt = (DateTimeOffset.UtcNow - _startTime).TotalMilliseconds;
                            OnElapsed?.Invoke((int)rtt);
                        }

                        _tcs.TrySetResult(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        Connection.Disconnect();
                    }

                    await Task.Delay(_options.Interval);
                }
            });
        }
    }

    protected override void Disposing()
    {
        _tcs?.TrySetResult(true);
        _tcs = null!;

        Connection.OnStanza -= OnStanza;
    }

    void OnStanza(Stanza stz)
    {
        if (stz is Iq iq)
        {
            // some servers just reply result without query element inside IQ

            if (_options.UseClientPingRequests && iq.Id == _stanzaId)
            {
                _tcs.TrySetResult(true);
                return;
            }

            if (iq.Query is Ping)
            {
                iq.Type = IqType.Result;
                iq.SwitchDirection();
                Connection.Send(iq);
            }
        }
    }
}