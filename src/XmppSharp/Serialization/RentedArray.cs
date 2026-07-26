using System.Buffers;

namespace XmppSharp.Serialization;

public sealed class RentedArray<T>(int size, bool clearArray = false) : IDisposable
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

	void ThrowIfDisposed()
	{
#if NET7_0_OR_GREATER
		ObjectDisposedException.ThrowIf(_disposed, this);
#else
		if (_disposed)
			throw new ObjectDisposedException(nameof(RentedArray<>));
#endif
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;

			ArrayPool<T>.Shared.Return(_buffer, clearArray);
		}
	}

	public static implicit operator Span<T>(RentedArray<T> array) => array.Span;

	public static implicit operator Memory<T>(RentedArray<T> array) => array.Memory;
}
