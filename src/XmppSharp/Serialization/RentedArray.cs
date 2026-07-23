using System.Buffers;

namespace XmppSharp.Serialization;

public ref struct RentedArray<T>(int size, bool clearArray = false) : IDisposable
{
	bool _disposed;

	T[] _buffer = ArrayPool<T>.Shared.Rent(size);

	public readonly Span<T> Span
	{
		get
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(RentedArray<>));

			return _buffer.AsSpan(0, size);
		}
	}

	public readonly Memory<T> Memory
	{
		get
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(RentedArray<>));

			return _buffer.AsMemory(0, size);
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			ArrayPool<T>.Shared.Return(_buffer, clearArray);
			_buffer = default!;
		}
	}
}
