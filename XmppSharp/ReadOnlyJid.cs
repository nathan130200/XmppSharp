using System.Diagnostics;

namespace XmppSharp;

/// <summary>
/// Immutable version of <see cref="Jid"/>, has no setters instead init-only properties.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct ReadOnlyJid : IEquatable<ReadOnlyJid>
{
	/// <inheritdoc cref="Jid.Empty"/>
	public static ReadOnlyJid Empty => new();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Jid _value;

	/// <inheritdoc cref="Jid(string)"/>
	public ReadOnlyJid()
		=> this._value = new();

	public ReadOnlyJid(Jid other) => this._value = new()
	{
		_local = other._local,
		_domain = other._domain,
		_resource = other._resource
	};

	/// <inheritdoc cref="Jid(string)"/>
	public ReadOnlyJid(string jid)
		=> this._value = new(jid);

	/// <inheritdoc cref="Jid(string, string, string)"/>
	public ReadOnlyJid(string local, string domain, string resource)
		=> this._value = new(local, domain, resource);

	/// <inheritdoc cref="Jid.Local"/>
	public string? Local
	{
		get => this._value.Local;
		init => this._value.Local = value;
	}

	/// <inheritdoc cref="Jid.Domain"/>
	public string Domain
	{
		get => this._value.Domain;
		init => this._value.Domain = value;
	}

	/// <inheritdoc cref="Jid.Resource"/>
	public string? Resource
	{
		get => this._value.Resource;
		init => this._value.Resource = value;
	}

	/// <inheritdoc cref="Jid.ToString"/>
	public override string ToString()
		=> this._value.ToString();

	public override int GetHashCode()
		=> this._value.GetHashCode();

	public override bool Equals(object? obj)
	{
		if (obj is not ReadOnlyJid other)
			return false;

		return this.Equals(other);
	}

	public bool Equals(ReadOnlyJid other)
	{
		if (this._value is null)
			return other._value is null;

		return this._value.Equals(other._value);
	}

	public static bool operator ==(ReadOnlyJid lhs, ReadOnlyJid rhs)
		=> lhs.Equals(rhs);

	public static bool operator !=(ReadOnlyJid lhs, ReadOnlyJid rhs)
		=> !(lhs == rhs);

	/// <summary>
	/// Converts between JID and ReadOnlyJID
	/// </summary>
	public static implicit operator Jid(ReadOnlyJid jid)
		=> jid._value;

	/// <summary>
	/// Converts between ReadOnlyJID and string.
	/// </summary>
	public static implicit operator string(ReadOnlyJid jid)
		=> jid._value.ToString();

	/// <summary>
	/// Converts between ReadOnlyJID and JID.
	/// </summary>
	public static implicit operator ReadOnlyJid(Jid jid)
		=> new(jid);
}