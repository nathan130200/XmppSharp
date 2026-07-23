using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace XmppSharp;

public sealed class Jid : IEquatable<Jid>
{
	readonly string _value;
	readonly int _at, _slash;
	readonly int _hashCode;

	public ReadOnlySpan<char> User
		=> _at != -1 ? _value.AsSpan(0.._at) : [];

	public ReadOnlySpan<char> Server
	{
		get
		{
			if (_at == -1 && _slash == -1)
				return _value;

			if (_slash == -1)
				return _value.AsSpan((_at + 1)..);

			return _value.AsSpan((_at + 1).._slash);
		}
	}

	public ReadOnlySpan<char> Resource
		=> _slash != -1 ? _value.AsSpan((_slash + 1)..) : [];

	public static Jid Empty { get; } = new();

	public Jid()
	{
		_value = string.Empty;
		_hashCode = 0;
		_at = _slash = -1;
	}

	public Jid(string jid)
	{
		ArgumentNullException.ThrowIfNull(jid);

		_value = jid;

		_at = _value.IndexOf('@');

		_slash = _value.IndexOf('/', _at + 1);

		_hashCode = HashCode.Combine(
				string.GetHashCode(User, StringComparison.OrdinalIgnoreCase),
				string.GetHashCode(Server, StringComparison.OrdinalIgnoreCase),
				string.GetHashCode(Resource, StringComparison.Ordinal));
	}

	public Jid(ReadOnlySpan<char> user, ReadOnlySpan<char> server, ReadOnlySpan<char> resource)
	{
		var size = server.Length
			+ (user.IsEmpty ? 0 : user.Length + 1)
			+ (resource.IsEmpty ? 0 : resource.Length + 1);

		_at = _slash = -1;

		if (size == 0)
		{
			_hashCode = 0;
			_value = string.Empty;
			return;
		}

		var buf = ArrayPool<char>.Shared.Rent(size);

		try
		{
			var pos = 0;

			if (!user.IsEmpty)
			{
				user.CopyTo(buf.AsSpan(pos));

				pos += user.Length;

				_at = pos;

				buf[pos++] = '@';
			}

			if (!server.IsEmpty)
			{
				server.CopyTo(buf.AsSpan(pos));

				pos += server.Length;
			}

			if (!resource.IsEmpty)
			{
				_slash = pos;

				buf[pos++] = '/';

				resource.CopyTo(buf.AsSpan(pos));

				pos += resource.Length;
			}

			_value = new string(buf, 0, pos);

			_hashCode = HashCode.Combine(
				string.GetHashCode(User, StringComparison.OrdinalIgnoreCase),
				string.GetHashCode(Server, StringComparison.OrdinalIgnoreCase),
				string.GetHashCode(Resource, StringComparison.Ordinal));
		}
		finally
		{
			ArrayPool<char>.Shared.Return(buf);
		}
	}

	public bool IsBare => _slash == -1;

	public bool IsServer => _at == -1;

	public override string ToString() => _value;

	public override int GetHashCode() => _hashCode;

	public override bool Equals(object? obj)
		=> obj is Jid other && Equals(other);

	public bool Equals(Jid? other)
	{
		if (other is null) return false;

		if (ReferenceEquals(_value, other._value)) return true;

		return User.Equals(other.User, StringComparison.OrdinalIgnoreCase)
			&& Server.Equals(other.Server, StringComparison.OrdinalIgnoreCase)
			&& Resource.Equals(other.Resource, StringComparison.Ordinal);
	}

	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator Jid?(string? jid) => jid is null ? null : new(jid);

	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator string?(Jid? jid) => jid?.ToString();
}
