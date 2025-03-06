using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using XmppSharp.Collections;

namespace XmppSharp;

/// <summary>
/// Represents the jabber identifier. <c>local@domain/resource</c>
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed record Jid : IEquatable<Jid>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string? _local, _resource;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _domain = default!;

    public Jid(Jid other)
    {
        _local = other._local;
        _domain = other._domain;
        _resource = other._resource;
    }

    public Jid(string? local, string domain, string? resource)
    {
        Local = local;
        Domain = domain;
        Resource = resource;
    }

    public Jid(string jid)
    {
        var at = jid.IndexOf('@');
        var slash = jid.IndexOf('/');

        if (at > 0)
            _local = jid[0..at];

        if (slash == -1)
            _domain = jid[(at + 1)..];
        else
        {
            _domain = jid[(at + 1)..slash];
            _resource = jid[(slash + 1)..];
        }
    }

    /// <summary>
    /// Local part
    /// </summary>
    public string? Local
    {
        get => _local;
        init => _local = value;
    }

    /// <summary>
    /// Domain part
    /// </summary>
    public string Domain
    {
        get => _domain;
        init
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (Uri.CheckHostName(value) == UriHostNameType.Unknown)
                    throw new InvalidOperationException();
            }

            _domain = value;
        }
    }

    /// <summary>
    /// Resource part
    /// </summary>
    public string? Resource
    {
        get => _resource;
        init => _resource = value;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (_local != null)
            sb.Append(_local).Append('@');

        if (_domain != null)
            sb.Append(_domain);

        if (_resource != null)
            sb.Append('/').Append(_resource);

        return sb.ToString();
    }

    [return: NotNullIfNotNull(nameof(jid))]
    public static implicit operator string?(Jid? jid) => jid?.ToString();

    [return: NotNullIfNotNull(nameof(jid))]
    public static implicit operator Jid?(string? jid)
    {
        if (jid == null)
            return null;

        return new(jid);
    }


    public bool IsBare
        => string.IsNullOrWhiteSpace(_resource);

    public bool IsServer
        => string.IsNullOrWhiteSpace(_local) && string.IsNullOrWhiteSpace(_resource);

    public Jid Bare => this with { Resource = null };

    public bool Equals(Jid? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(other, this))
            return true;

        return FullJidComparer.AreEquals(this, other);
    }

    public override int GetHashCode() => HashCode.Combine
    (
        Local?.GetHashCode() ?? 0,
        Domain?.GetHashCode() ?? 0,
        Resource?.GetHashCode() ?? 0
    );
}
