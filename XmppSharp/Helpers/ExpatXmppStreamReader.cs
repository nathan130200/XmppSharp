using System.Diagnostics;
using System.Runtime.CompilerServices;
using Expat;
using XmppSharp.Entities.Options;

namespace XmppSharp.Helpers;

public class ExpatXmppStreamReader : XmppStreamReader
{
    private ExpatXmppParser? _parser;
    private volatile bool _reading = false;
    private TaskCompletionSource _tcs;

    public ExpatXmppStreamReader(XmppConnectionOptions options) : base(options)
    {
        _parser = new ExpatXmppParser(ExpatEncoding.UTF8);
        _parser.OnStreamStart += e => FireOnStreamStart(e);
        _parser.OnStreamElement += e => FireOnStreamElement(e);
        _parser.OnStreamEnd += () => FireOnStreamEnd();
    }

    public override void Pause()
    {
        _reading = false;
    }

    public override TaskAwaiter GetAwaiter()
        => _tcs.Task.GetAwaiter();

    private Stream _stream;

    public override void Reset(Stream stream, CancellationToken token)
    {
        ThrowIfDisposed();

        Debug.Assert(stream != null);

        _tcs = new TaskCompletionSource();

        _parser!.Reset();
        _stream = stream;

        if (!_reading)
        {
            _reading = true;
            _ = BeginRead(token);
        }
    }

    async Task BeginRead(CancellationToken token)
    {
        var buf = new byte[Options.RecvBufferSize];

        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(16, token);

                if (!_reading)
                    break;

                int len = await _stream!.ReadAsync(buf, token);
                _parser!.Parse(buf, len, len == 0);

                if (len <= 0)
                {
                    Dispose();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _tcs?.TrySetException(ex);
            FireOnError(ex);
            Dispose();
        }
        finally
        {
            _tcs?.TrySetResult();
            _reading = false;
        }
    }

    protected override void Disposing()
    {
        _parser?.Dispose();
        _parser = null;
    }
}