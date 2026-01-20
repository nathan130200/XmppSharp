using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace XmppSharp;

/// <summary>
/// Represents an jabber identifier.
/// <list type="bullet">
/// <item><c>local@domain</c></item>
/// <item><c>local@domain/resource</c></item>
/// <item><c>domain/resource</c></item>
/// </list>
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class Jid : IEquatable<Jid>
{
	readonly Lazy<string?>? _toStringLazy;

	public static readonly Jid Empty = new();

	Jid()
	{

	}

	public Jid(Jid other)
	{
		_toStringLazy = new(BuildCache, true);

		Local = other.Local;
		Domain = other.Domain;
		Resource = other.Resource;
	}

	public Jid(string? local = default, string? domain = default, string? resource = default)
	{
		_toStringLazy = new(BuildCache, true);

		Local = local;
		Domain = domain;
		Resource = resource;
	}

	public Jid(string jid)
	{
		_toStringLazy = new(BuildCache, true);

		var at = jid.IndexOf('@');
		var slash = jid.IndexOf('/');

		if (at > 0)
			Local = jid[0..at];

		if (slash == -1)
			Domain = jid[(at + 1)..];
		else
		{
			Domain = jid[(at + 1)..slash];
			Resource = jid[(slash + 1)..];
		}
	}

	public string? Local { get; init; }

	public string? Domain
	{
		get => field;
		init
		{
			if (!string.IsNullOrWhiteSpace(value) && Uri.CheckHostName(value) == UriHostNameType.Unknown)
				throw new ArgumentException(null, nameof(Domain));

			field = value;
		}
	}

	public string? Resource { get; init; }

	string? BuildCache()
	{
		if (Local == null && Domain == null && Resource == null)
			return null;

		var sb = new StringBuilder();

		if (Local != null)
			sb.Append(Local).Append('@');

		sb.Append(Domain);

		if (Resource != null)
			sb.Append('/').Append(Resource);

		return sb.ToString();
	}

	[return: MaybeNull]
	public override string ToString() => _toStringLazy?.Value;

	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator string?(Jid? jid) => jid?.ToString();

	[return: NotNullIfNotNull(nameof(jid))]
	public static implicit operator Jid?(string? jid)
	{
		if (jid is null)
			return null;

		return new(jid);
	}


	public bool IsBare
		=> string.IsNullOrWhiteSpace(Resource);

	public bool IsServer
		=> string.IsNullOrWhiteSpace(Local) && string.IsNullOrWhiteSpace(Resource);

	public Jid Bare => new(Local, Domain);

	public override bool Equals(object? obj)
		=> obj is Jid other && Equals(other);

	public bool Equals(Jid? other)
	{
		if (other is null) return false;

		if (ReferenceEquals(this, other)) return true;

		return string.Equals(Local, other.Local, StringComparison.OrdinalIgnoreCase)
			&& string.Equals(Domain, other.Domain, StringComparison.OrdinalIgnoreCase)
			&& string.Equals(Resource, other.Resource, StringComparison.Ordinal);
	}

	public override int GetHashCode()
	{
		var hash = new HashCode();
		hash.Add(Local, StringComparer.OrdinalIgnoreCase);
		hash.Add(Domain, StringComparer.OrdinalIgnoreCase);
		hash.Add(Resource, StringComparer.Ordinal);
		return hash.ToHashCode();
	}

	public static bool operator !=(Jid? x, Jid? y) => !(x == y);

	public static bool operator ==(Jid? x, Jid? y)
	{
		if (x is null && y is null) return true;

		if (x is null || y is null) return false;

		return x.Equals(y);
	}
}
