using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace XmppSharp;

/// <summary>
/// Represents a jabber identifier.
/// </summary>
public sealed class Jid : IEquatable<Jid>
{
	static readonly SearchValues<char> s_EscapeNodeChars = SearchValues.Create("\\ \"&'/:<>@");
	readonly string _value;
	readonly int _at, _slash, _hashCode;

	/// <summary>Gets a empty instance of JID.</summary>
	public static Jid Empty { get; } = new();

	Jid()
	{
		_value = string.Empty;
		_at = _slash = -1;
		_hashCode = 0;
	}

	/// <summary>Initialize a new instance of <see cref="Jid"/>.</summary>
	/// <param name="jid">String to parse.</param>
	public Jid(string jid)
	{
		ArgumentNullException.ThrowIfNull(jid);

		_value = jid;

		_at = _value.IndexOf('@');

		if (_at == -1)
			_slash = _value.IndexOf('/');
		else
			_slash = _value.IndexOf('/', _at);

		_hashCode = BuildHashCode(User, Server, Resource);
	}

	static void ValidateUser(ReadOnlySpan<char> user)
	{
		if (user.ContainsAny(s_EscapeNodeChars))
			throw new ArgumentException(default, nameof(user));
	}

	static void ValidateServer(ReadOnlySpan<char> server)
	{
		if (server.ContainsAny("@/"))
			throw new ArgumentException(default, nameof(server));
	}

	/// <summary>Initialize a new instance of <see cref="Jid"/>.</summary>
	/// <param name="user">Local part of JID.</param>
	/// <param name="server">Domain part of JID.</param>
	/// <param name="resource">Resource part of JID.</param>
	public Jid(ReadOnlySpan<char> user, ReadOnlySpan<char> server, ReadOnlySpan<char> resource)
	{
		_at = _slash = -1;

		var size = server.Length
			+ (!user.IsEmpty ? user.Length + 1 : 0)
			+ (!resource.IsEmpty ? resource.Length + 1 : 0);

		ValidateUser(user);
		ValidateServer(server);

		if (size == 0)
		{
			_hashCode = 0;
			_value = string.Empty;
			return;
		}

		var buf = ArrayPool<char>.Shared.Rent(size);

		try
		{
			var ofs = 0;

			if (!user.IsEmpty)
			{
				user.CopyTo(buf.AsSpan(ofs));

				ofs += user.Length;

				_at = ofs++;

				buf[_at] = '@';
			}

			if (!server.IsEmpty)
			{
				server.CopyTo(buf.AsSpan(ofs));

				ofs += server.Length;
			}

			if (!resource.IsEmpty)
			{
				_slash = ofs++;

				buf[_slash] = '/';

				resource.CopyTo(buf.AsSpan(ofs));
			}

			_value = new string(buf.AsSpan(0, ofs));

			_hashCode = BuildHashCode(User, Server, Resource);
		}
		finally
		{
			ArrayPool<char>.Shared.Return(buf);
		}
	}

	/// <summary>Gets the local part of JID.</summary>
	public ReadOnlySpan<char> User
		=> _at != -1 ? _value.AsSpan(0.._at) : [];

	/// <summary>Gets the domain part of JID.</summary>
	public ReadOnlySpan<char> Server
	{
		get
		{
			if (_at == -1 && _slash == -1) return _value;

			var start = _at != -1 ? (_at + 1) : 0;
			var end = _slash != -1 ? _slash : ^0;
			return _value.AsSpan(start..end);
		}
	}

	/// <summary>Gets the resource of JID.</summary>
	public ReadOnlySpan<char> Resource
		=> _slash != -1 ? _value.AsSpan((_slash + 1)..) : [];

	/// <summary>Gets the computed hash code of this JID.</summary>
	public override int GetHashCode() => _hashCode;

	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is Jid other && Equals(other);

	/// <inheritdoc/>
	public bool Equals(Jid? other)
	{
		if (other is null) return false;

		if (ReferenceEquals(_value, other._value)) return true;

		return User.Equals(other.User, StringComparison.OrdinalIgnoreCase)
			&& Server.Equals(other.Server, StringComparison.OrdinalIgnoreCase)
			&& Resource.Equals(other.Resource, StringComparison.Ordinal);
	}

	/// <summary>Gets JID instance as string representation.</summary>
	public override string ToString() => _value;

	static int BuildHashCode(ReadOnlySpan<char> user, ReadOnlySpan<char> server, ReadOnlySpan<char> resource)
	{
		if (user.IsEmpty && server.IsEmpty && resource.IsEmpty) return 0;

		var state = new HashCode();
		state.Add(string.GetHashCode(user, StringComparison.OrdinalIgnoreCase));
		state.Add(string.GetHashCode(server, StringComparison.OrdinalIgnoreCase));
		state.Add(string.GetHashCode(resource, StringComparison.Ordinal));
		return state.ToHashCode();
	}

	/// <summary>Implicit convert string to JID.</summary>
	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator Jid?(string? jid)
	{
		if (jid is null) return null;

		return new Jid(jid);
	}

	/// <summary>Implicit convert JID to string.</summary>
	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator string?(Jid? jid) => jid?.ToString();

	/// <inheritdoc />
	public static bool operator !=(Jid? x, Jid? y) => !(x == y);

	/// <inheritdoc />
	public static bool operator ==(Jid? x, Jid? y)
	{
		if (ReferenceEquals(x, y)) return true;

		if (x is null) return y is null;

		return x.Equals(y);
	}
}
