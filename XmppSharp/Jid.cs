using System.Diagnostics;
using System.Text;

namespace XmppSharp;

[DebuggerDisplay("{ToString(),nq}")]
public class Jid
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _local, _domain, _resource;

    public string Local
    {
        get => _local;
        set => _local = value?.ToLowerInvariant();
    }

    public string Domain
    {
        get => _domain;
        set
        {
            value = value?.ToLowerInvariant();

            if (Uri.CheckHostName(value) == UriHostNameType.Unknown)
                throw new ArgumentException("Invalid domain.", nameof(value));

            _domain = value;
        }
    }

    public string Resource
    {
        get => _resource;
        set => _resource = value;
    }

    Jid()
    {

    }

    public Jid(string domain)
        => Domain = domain;

    public Jid(string local, string domain, string resource)
        => (Local, Domain, Resource) = (local, domain, resource);

    public static Jid Parse(string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        if (!TryParse(input, out var result))
            throw new FormatException("Invalid jid.");

        return result;
    }

    static readonly char[] JidTokens = { '@', '/' };

    static bool TryParseComponents(string input, out string local, out string domain, out string resource)
    {
        local = default; domain = default; resource = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.IndexOfAny(JidTokens) == -1)
        {
            domain = input;
            return true;
        }
        else
        {
            var at = input.IndexOf(JidTokens[0]);

            if (at != -1)
                local = input[0..at];

            var slash = input.IndexOf(JidTokens[1]);

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

    public override int GetHashCode()
        => HashCode.Combine(_local?.GetHashCode(), _domain?.GetHashCode(), _resource?.GetHashCode());

    public override bool Equals(object? obj)
    {
        if (obj is not Jid other)
            return false;

        if (IsBare && other.IsBare)
            return IsBareEquals(this, other);

        return IsFullEqual(this, other);
    }

    public static bool IsBareEquals(Jid lhs, Jid rhs)
    {
        if (lhs is null || rhs is null)
            return false;

        if (!lhs.IsBare || !rhs.IsBare)
            return false;

        return string.Equals(lhs._local, rhs._local, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._domain, rhs._domain, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFullEqual(Jid lhs, Jid rhs)
    {
        if (lhs is null || rhs is null)
            return false;

        if (lhs.IsBare || rhs.IsBare)
            return false;

        return string.Equals(lhs._local, rhs._local, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._domain, rhs._domain, StringComparison.OrdinalIgnoreCase)
             && string.Equals(lhs._resource, rhs.Resource, StringComparison.Ordinal);
    }

    public static bool operator ==(Jid lhs, Jid rhs)
        => lhs?.Equals(rhs) == true;

    public static bool operator !=(Jid lhs, Jid rhs)
        => !(lhs == rhs);

    public static implicit operator Jid(string s)
    {
        if (TryParse(s, out var jid))
            return jid;

        return null;
    }

    public static implicit operator string(Jid j)
        => j.ToString();
}
