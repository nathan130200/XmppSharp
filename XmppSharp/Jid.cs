using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace XmppSharp;

[DebuggerDisplay("{ToString(),nq}")]
public sealed record Jid : IEquatable<Jid>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal string _local, _domain, _resource;

    public string? Local
    {
        get => _local;
        set => _local = EnsureByteSize(value?.ToLowerInvariant());
    }

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

    internal Jid()
    {

    }

    public static Jid Empty => new();

    public Jid(string jid)
    {
        if (!TryParseComponents(jid, out _local, out _domain, out _resource))
            Domain = jid;
    }

    public Jid(string? local, string domain, string? resource)
        => (Local, Domain, Resource) = (local, domain, resource);

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

    public bool IsBare
        => _resource is null;

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

    public static bool IsBareEquals(Jid lhs, Jid rhs)
    {
        if (lhs is null)
            return rhs is null;

        if (rhs is null)
            return false;

        return string.Equals(lhs._local, rhs._local, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._domain, rhs._domain, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFullEqual(Jid lhs, Jid rhs)
    {
        if (lhs is null)
            return rhs is null;

        if (rhs is null)
            return false;

        return string.Equals(lhs._local, rhs._local, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._domain, rhs._domain, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._resource, rhs.Resource, StringComparison.Ordinal);
    }

    public bool Equals(Jid other)
        => IsFullEqual(this, other);

    public static implicit operator Jid(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        if (TryParse(s, out var jid))
            return jid;

        return null;
    }

    public static implicit operator string(Jid j)
        => j.ToString();
}
