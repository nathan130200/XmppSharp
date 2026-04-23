using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using XmppSharp.Collections;

namespace XmppSharp;

#pragma warning disable

/// <summary>
/// Defines an interface for comparing Jid instances, providing methods for both equality comparison and ordering comparison.
/// </summary>
public interface JidComparer : IEqualityComparer<Jid>, IComparer<Jid>
{

}

#pragma warning restore

/// <summary>
/// Represents a Jabber ID (JID) in the XMPP protocol.
/// </summary>
public sealed class Jid : IEquatable<Jid>
{
    /// <summary>
    /// Defines the maximum allowed length for each component of a JID (local, domain, resource).
    /// </summary>
    public const int MaxComponentSize = 1023;

    /// <summary>
    /// Defines the maximum allowed length for a JID string.
    /// </summary>
    public const int MaxJidSize = (MaxComponentSize * 3) + 2;

    /// <summary>
    /// Gets a comparer for JID instances that compares only the local and domain parts.
    /// </summary>
    public static JidComparer BareJidComparer { get; } = new BareJidComparer();

    /// <summary>
    /// Gets a comparer for JID instances that compares the local, domain, and resource parts.
    /// </summary>
    public static JidComparer FullJidComparer { get; } = new FullJidComparer();

    /// <summary>
    /// Gets the local value associated with this instance.
    /// </summary>
    public string? Local
    {
        get => field;

        init
        {
            if (value != null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, MaxComponentSize);

            field = value;
        }
    }

    /// <summary>
    /// Gets the domain associated with the current instance.
    /// </summary>
    public string? Domain
    {
        get => field;

        init
        {
            if (value == null)
                field = null;
            else
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, MaxComponentSize);

                if (Uri.CheckHostName(value) == UriHostNameType.Unknown)
                    throw new ArgumentException($"The domain part of a JID must be a valid hostname.", nameof(value));

                field = string.Intern(value);
            }
        }
    }

    /// <summary>
    /// Gets the resource associated with the current jid.
    /// </summary>
    public string? Resource
    {
        get => field;

        init
        {
            if (value != null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, MaxComponentSize);

            field = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the Jid class by copying the values from an existing Jid instance.
    /// </summary>
    /// <param name="other">The Jid instance whose values are copied to initialize the new instance. Cannot be null.</param>
    public Jid(Jid other) : this()
    {
        ArgumentNullException.ThrowIfNull(other);

        Local = other.Local;
        Domain = other.Domain;
        Resource = other.Resource;
    }


    /// <summary>
    /// Gets an empty JID instance. This can be used as a default value or to represent an uninitialized JID.
    /// </summary>
    public static Jid Empty { get; } = new();

    /// <summary>
    /// Initializes a new instance of the Jid class with empty values.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public Jid()
    {

    }

    /// <summary>
    /// Initializes a new instance of the Jid class with the specified local part, domain, and resource.
    /// </summary>
    /// <param name="local">The local part of the JID.</param>
    /// <param name="domain">The domain part of the JID.</param>
    /// <param name="resource">The resource part of the JID.</param>
    [OverloadResolutionPriority(0)]
    public Jid(string? local = default, string? domain = default, string? resource = default) : this()
    {
        Local = local;
        Domain = domain;
        Resource = resource;
    }

    string? _toString;

    /// <summary>
    /// Initializes a new instance of the Jid class by parsing the specified Jabber ID (JID) string into its local,
    /// domain, and resource components.
    /// </summary>
    /// <remarks>If the JID does not contain a resource part, only the local and domain components are set.
    /// The constructor does not validate the format beyond splitting the string; callers should ensure the input is a
    /// valid JID.</remarks>
    /// <param name="jid">The Jabber ID (JID) string to parse. Must be in the format 'local@domain/resource', where the local and resource
    /// parts are optional.</param>
    public Jid(string jid)
    {
        var at = jid.IndexOf('@');

        if (at > 0)
            Local = jid[0..at];

        var slash = jid.IndexOf('/');

        if (slash > 0)
        {
            Domain = jid[(at + 1)..slash];
            Resource = jid[(slash + 1)..];
        }
        else
        {
            Domain = jid[(at + 1)..];
        }

        _toString = jid;
    }

    /// <summary>
    /// Determines whether the JID represents a server or a client. A JID is considered a server if it does not have a local part.
    /// </summary>
    public bool IsServer => string.IsNullOrWhiteSpace(Local);

    /// <summary>
    /// Gets a value indicating whether the identifier is bare, meaning it does not include a resource part.
    /// </summary>
    public bool IsBare => !string.IsNullOrWhiteSpace(Resource);

    /// <summary>
    /// Returns a string that represents the current JID (Jabber Identifier).
    /// </summary>
    /// <returns>A string representation of the current JID.</returns>
    public override string ToString()
    {
        lock (this)
        {
            if (_toString == null)
            {
                if (Local == null && Domain == null && Resource == null)
                    return string.Empty;

                var sb = new StringBuilder();

                if (Local != null)
                    sb.Append(Local).Append('@');

                sb.Append(Domain);

                if (Resource != null)
                    sb.Append('/').Append(Resource);

                _toString = sb.ToString();
            }

            return _toString;
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Local, StringComparer.OrdinalIgnoreCase);
        hash.Add(Domain, StringComparer.OrdinalIgnoreCase);
        hash.Add(Resource, StringComparer.Ordinal);
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Jid other && Equals(other);
    }

    /// <summary>
    /// Determines whether the current Jid instance is equal to the specified Jid instance.
    /// </summary>
    /// <remarks>Comparison is case-insensitive for the local and domain parts, and case-sensitive for the
    /// resource part.</remarks>
    /// <param name="other">The Jid instance to compare with the current instance. May be null.</param>
    /// <returns>true if the specified Jid is not null and has the same local, domain, and resource parts as the current
    /// instance; otherwise, false.</returns>
    public bool Equals(Jid? other)
    {
        if (other is null) return false;

        return string.Equals(Local, other.Local, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Domain, other.Domain, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Resource, other.Resource, StringComparison.Ordinal);
    }

    /// <summary>
    /// Converts a string representation of a JID to a Jid instance.
    /// </summary>
    /// <remarks>This operator enables implicit conversion from a string to a Jid. If the input string is
    /// null, the result is null; otherwise, a new Jid instance is created from the string.</remarks>
    /// <param name="jid">The string representation of the JID to convert. Can be null.</param>
    [return: NotNullIfNotNull(nameof(jid))]
    public static implicit operator Jid?(string? jid)
    {
        if (jid is null) return null;
        return new Jid(jid);
    }

    /// <summary>
    /// Defines an implicit conversion from a Jid instance to its string representation.
    /// </summary>
    /// <remarks>If the specified Jid is null, the result is null; otherwise, the result is the string
    /// representation of the Jid.</remarks>
    /// <param name="jid">The Jid instance to convert. Can be null.</param>
    [return: MaybeNull]
    public static implicit operator string?(Jid? jid) => jid?.ToString();

    /// <summary>
    /// Determines whether two specified Jid instances are not equal.
    /// </summary>
    /// <param name="x">The first Jid instance to compare, or null.</param>
    /// <param name="y">The second Jid instance to compare, or null.</param>
    /// <returns>true if the specified Jid instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Jid? x, Jid? y) => !(x == y);

    /// <summary>
    /// Determines whether two Jid instances are equal.
    /// </summary>
    /// <param name="x">The first Jid instance to compare, or null.</param>
    /// <param name="y">The second Jid instance to compare, or null.</param>
    /// <returns>true if both Jid instances are equal or both are null; otherwise, false.</returns>
    public static bool operator ==(Jid? x, Jid? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }
}
