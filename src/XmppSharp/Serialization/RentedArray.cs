using System.Buffers;

namespace XmppSharp.Serialization;

public ref struct RentedArray<T>(int size, bool clearArray = false) : IDisposable
{
    bool _disposed;

    readonly T[] _buffer = ArrayPool<T>.Shared.Rent(size);

    public Span<T> Span
    {
        get
        {
            ThrowIfDisposed();
            return _buffer.AsSpan(0, size);
        }
    }

    public Memory<T> Memory
    {
        get
        {
            ThrowIfDisposed();
            return _buffer.AsMemory(0, size);
        }
    }

    readonly void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RentedArray<>));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            ArrayPool<T>.Shared.Return(_buffer, clearArray);
        }
    }
}
