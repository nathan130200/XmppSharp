using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace XmppSharp;

public sealed class Jid : IEquatable<Jid>
{
	readonly string _value;
	readonly int _at, _slash, _hashCode;

	public Jid(string jid)
	{
		ArgumentNullException.ThrowIfNull(jid);

		_value = jid;

		_at = _value.IndexOf('@');

		_slash = _value.IndexOf('/');

		_hashCode = BuildHashCode(User, Server, Resource);
	}

	public Jid(ReadOnlySpan<char> user, ReadOnlySpan<char> server, ReadOnlySpan<char> resource)
	{
		_at = _slash = -1;

		var size = server.Length
			+ (user.Length != 0 ? 1 + user.Length : 0)
			+ (resource.Length != 0 ? 1 + resource.Length : 0);

		if (size == 0)
			_value = string.Empty;
		else
		{
			var buf = ArrayPool<char>.Shared.Rent(size);

			var pos = 0;

			try
			{
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
					buf[pos] = '/';

					_slash = pos++;

					resource.CopyTo(buf.AsSpan(pos));

					pos += resource.Length;
				}

				_value = new string(buf, 0, pos);

				_hashCode = BuildHashCode(User, Server, Resource);
			}
			finally
			{
				ArrayPool<char>.Shared.Return(buf);
			}
		}
	}

	static int BuildHashCode(ReadOnlySpan<char> user, ReadOnlySpan<char> server, ReadOnlySpan<char> resource)
	{
		var state = new HashCode();
		state.Add(string.GetHashCode(user, StringComparison.OrdinalIgnoreCase));
		state.Add(string.GetHashCode(server, StringComparison.OrdinalIgnoreCase));
		state.Add(string.GetHashCode(resource, StringComparison.Ordinal));
		return state.ToHashCode();
	}

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

	public bool IsBare => _slash == -1;

	public bool IsServer => _at == -1;

	public Jid Bare
	{
		get
		{
			if (IsBare)
				return this;

			return new(User, Server, default);
		}
	}

	public override int GetHashCode() => _hashCode;

	public override bool Equals(object? obj) => obj is Jid other && Equals(other);

	public override string ToString() => _value;

	public bool Equals(Jid? other)
	{
		if (other is null) return false;

		if (ReferenceEquals(_value, other._value)) return true;

		return User.Equals(other.User, StringComparison.OrdinalIgnoreCase)
			&& Server.Equals(other.Server, StringComparison.OrdinalIgnoreCase)
			&& Resource.Equals(other.Resource, StringComparison.Ordinal);
	}

	public static bool operator !=(Jid? x, Jid? y) => !(x == y);

	public static bool operator ==(Jid? x, Jid? y)
	{
		if (x is null) return y is null;

		return x.Equals(y);
	}

	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator string?(Jid? jid) => jid?.ToString();

	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator Jid?(string? jid) => jid is null ? null : new(jid);
}
