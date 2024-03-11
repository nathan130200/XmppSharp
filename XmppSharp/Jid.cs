using System.Text;

namespace XmppSharp;

public record class Jid
{
    private readonly string _local, _domain, _resource;

    public string Local
    {
        get => _local;
        init => _local = value?.ToLowerInvariant();
    }

    public required string Domain
    {
        get => _domain;
        init
        {
            value = value.ToLowerInvariant();

            if (Uri.CheckHostName(value) != UriHostNameType.Dns)
                throw new ArgumentException("Invalid hostname.", nameof(value));

            _domain = value;
        }
    }

    public string Resource
    {
        get => _resource;
        init => _resource = value;
    }

    public static Jid Parse(string input, bool thrown = true)
    {
        if (!thrown)
        {
            _ = TryParse(input, out var result);
            return result;
        }
        else
        {
            ArgumentException.ThrowIfNullOrEmpty(input);

            if (!TryParse(input, out var result))
                throw new FormatException();

            return result;
        }
    }

    public static bool TryParse(string input, out Jid result)
    {
        result = default;

        if (input.IndexOfAny(new[] { '@', '/' }, 0) == -1)
        {
            result = new Jid { Domain = input };
            return true;
        }
        else
        {
            string local = default, resource = default,
                domain;

            var at = input.IndexOf('@');

            if (at != -1)
                local = input[0..at];

            var slash = input.IndexOf('/');

            if (slash == -1)
                domain = input[(at + 1)..];
            else
            {
                domain = input[at..slash];
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
        }

        return result != null;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (_local != null)
            sb.Append(_local).Append('@');

        sb.Append(_domain);

        if (_resource != null)
            sb.Append('/').Append(_resource);


        return sb.ToString();
    }
}
