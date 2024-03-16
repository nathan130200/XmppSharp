using System.Diagnostics;
using System.Text;

namespace XmppSharp;

[DebuggerDisplay("{ToString(),nq}")]
public record class Jid
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _local, _domain, _resource;

    public string Local
    {
        get => _local;
        init => _local = value?.ToLowerInvariant();
    }

    public string Domain
    {
        get => _domain;
        init
        {
            value = value.ToLowerInvariant();

            if (Uri.CheckHostName(value) == UriHostNameType.Unknown)
                throw new ArgumentException("Invalid domain.", nameof(value));

            _domain = value;
        }
    }

    public string Resource
    {
        get => _resource;
        init => _resource = value;
    }

    public static Jid Parse(string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        if (!TryParse(input, out var result))
            throw new FormatException("Invalid jid.");

        return result;
    }

    private static readonly char[] JidSearchChars = { '@', '/' };

    public static bool TryParse(string input, out Jid result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.IndexOfAny(JidSearchChars) == -1)
        {
            result = new Jid { Domain = input };
            return true;
        }
        else
        {
            string local = default, resource = default,
                domain;

            var at = input.IndexOf(JidSearchChars[0]);

            if (at != -1)
                local = input[0..at];

            var slash = input.IndexOf(JidSearchChars[1]);

            if (slash == -1)
                domain = input[(at + 1)..];
            else
            {
                domain = input[(at + 1)..slash];
                resource = input[(slash + 1)..];
            }

            if (string.IsNullOrEmpty(domain))
                return false;

            result = new Jid
            {
                Local = local,
                Domain = domain,
                Resource = resource
            };

            return true;
        }
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

    public Jid Bare => this with
    {
        Resource = null
    };
}
