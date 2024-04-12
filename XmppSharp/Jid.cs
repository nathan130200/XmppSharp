using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace XmppSharp;

/// <summary>
/// Represents an jabber identifier.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed record Jid : IEquatable<Jid>
#if NET7_0_OR_GREATER
    , IParsable<Jid>
#endif

{

#if NET7_0_OR_GREATER

    /// <inheritdoc />
    public static bool TryParse(string? s, IFormatProvider? provider, out Jid result)
        => TryParse(s, out result);

    /// <inheritdoc />
    public static Jid Parse(string s, IFormatProvider? provider)
        => Parse(s);

#endif

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal string _local, _domain, _resource;

    /// <summary>
    /// Gets or sets the local part of the jid. <para>Generally contains the username.</para>
    /// </summary>
    public string? Local
    {
        get => _local;
        set => _local = EnsureByteSize(value?.ToLowerInvariant());
    }

    /// <summary>
    /// Gets or sets the part of the kid's domain.
    /// <para>
    /// It usually contains the hostname, address, or qualified name of a server or domain.
    /// </para>
    /// </summary>
    public string Domain
    {
        get => _domain;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException("Domain part cannot be null or empty.");

            _domain = EnsureByteSize(value);
        }
    }

    /// <summary>
    /// Gets or sets the resource part of the jid.
    /// <para>
    /// Contains a string that serves as a unique identifier to identify connections or 
    /// allow more than one access to the same user account and password.
    /// </para>
    /// </summary>
    public string? Resource
    {
        get => _resource;
        set => _resource = EnsureByteSize(value);
    }

    internal static string EnsureByteSize(string? s, [CallerMemberName] string param = null)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        int len;

        if ((len = Encoding.UTF8.GetByteCount(s)) > 1023)
            throw new ArgumentOutOfRangeException(param, len, $"{param} part exceeds the maximum bytes allowed.");

        return s;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="Jid"/>.
    /// </summary>
    public Jid()
    {

    }

    /// <summary>
    /// Creates an empty jid.
    /// </summary>
    public static Jid Empty => new();

    /// <summary>
    /// Initializes a new instance of <see cref="Jid" />.
    /// </summary>
    /// <param name="jid">String that will attempt to be validated to be valid JID, or by default will assign to <see cref="Domain" />.</param>
    public Jid(string jid)
    {
        if (!TryParseComponents(jid, out _local, out _domain, out _resource))
            Domain = jid;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Jid" />.
    /// </summary>
    /// <param name="local">Initial value for the local part.</param>
    /// <param name="domain">Initial value for the domain part.</param>
    /// <param name="resource">Initial value for the resource part.</param>
    public Jid(string? local, string domain, string? resource)
        => (Local, Domain, Resource) = (local, domain, resource);

    /// <summary>
    /// Try parsing or throw an exception if you encounter a problem.
    /// </summary>
    /// <param name="input">String that will attempt to be validated to be valid JID</param>
    /// <returns>JID instance.</returns>
    /// <exception cref="FormatException">When the supplied string is invalid.</exception>
    public static Jid Parse(string input)
    {
        Require.NotNullOrEmpty(input);

        if (!TryParse(input, out var result))
            throw new FormatException("Invalid jid.");

        return result;
    }

    internal const char LocalPart = '@';
    internal const char ResourcePart = '/';

    static bool TryParseComponents(string input, out string local, out string domain, out string resource)
    {
        local = default; domain = default; resource = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (!input.Contains(LocalPart) && !input.Contains(ResourcePart))
        {
            domain = input;
            return true;
        }
        else
        {
            var at = input.IndexOf(LocalPart);

            if (at != -1)
                local = input[0..at];

            var slash = input.IndexOf(ResourcePart);

            if (slash == -1)
                domain = input[(at + 1)..];
            else
            {
                domain = input[(at + 1)..slash];
                resource = input[(slash + 1)..];
            }

            if (string.IsNullOrEmpty(domain))
                return false;

            return Uri.CheckHostName(domain) != UriHostNameType.Unknown;
        }
    }

    /// <summary>
    /// It tries to parse the JID through the string, but it doesn't throw exceptions.
    /// </summary>
    /// <param name="input">String that will attempt to be validated to be valid JID</param>
    /// <param name="result">JID that was successfully parsed or <see langword="null" /> if it is invalid.</param>
    /// <returns><see langword="true" /> if the JID was successfully parsed, <see langword="false"/> otherwise.</returns>
    public static bool TryParse(string input, out Jid result)
    {
        result = default;

        if (TryParseComponents(input, out var local, out var domain, out var resource))
        {
            result = new Jid
            {
                _local = local,
                _domain = domain,
                _resource = resource
            };
        }

        return result != null;
    }

    /// <summary>
    /// Gets string representation of the JID in the XMPP form.
    /// </summary>
    public override string ToString()
    {
        var sb = StringBuilderPool.Rent();

        if (_local != null)
            sb.Append(_local).Append('@');

        if (_domain != null)
            sb.Append(_domain);

        if (_resource != null)
            sb.Append('/').Append(_resource);

        return StringBuilderPool.Return(sb);
    }

    /// <summary>
    /// Determines whether the current JID is bare, that is, it has no resource.
    /// </summary>
    public bool IsBare
        => _resource is null;

    /// <summary>
    /// Gets a copy of the current JID without the resource part.
    /// </summary>
    public Jid Bare => new()
    {
        _local = _local,
        _domain = _domain,
        _resource = null
    };

    public override int GetHashCode() => HashCode.Combine(
        _local?.GetHashCode() ?? 0,
        _domain?.GetHashCode() ?? 0,
        _resource?.GetHashCode() ?? 0);

    /// <summary>
    /// Determines whether both JIDs are the same "Bare" (i.e. they have no resource and the other components are the same)
    /// </summary>
    /// <param name="lhs">First JID to compare.</param>
    /// <param name="rhs">Second JID to compare.</param>
    /// <returns><see langword="true"/> if both JIDs are equals, otherwise <see langword="false" />.</returns>
    public static bool IsBareEquals(Jid lhs, Jid rhs)
    {
        if (lhs is null)
            return rhs is null;

        if (!lhs.IsBare || !rhs.IsBare)
            return false;

        return string.Equals(lhs._local, rhs._local, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._domain, rhs._domain, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether both JIDs are the same "Full" (i.e. both have resources and all components are the same)
    /// </summary>
    /// <param name="lhs">First JID to compare.</param>
    /// <param name="rhs">Second JID to compare.</param>
    /// <returns><see langword="true"/> if both JIDs are equals, otherwise <see langword="false" />.</returns>
    public static bool IsFullEqual(Jid lhs, Jid rhs)
    {
        if (lhs is null)
            return rhs is null;

        if (lhs.IsBare || rhs.IsBare)
            return false;

        return string.Equals(lhs._local, rhs._local, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._domain, rhs._domain, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._resource, rhs.Resource, StringComparison.Ordinal);
    }

    public bool Equals(Jid other)
        => IsFullEqual(this, other);

    /// <summary>
    /// Implicit convert string to JID.
    /// </summary>
    public static implicit operator Jid(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        if (TryParse(s, out var jid))
            return jid;

        return null;
    }

    /// <summary>
    /// Implicit convert JID to string.
    /// </summary>
    public static implicit operator string(Jid j)
        => j.ToString();
}
