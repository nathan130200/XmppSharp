using System.Text;

namespace XmppSharp;

public record class Jid : IParsable<Jid>
{
    public string User
    {
        get;
        init;
    }

    public required string Server
    {
        get;
        init;
    }

    public string Resource
    {
        get;
        init;
    }

    public Jid()
    {

    }

    static Jid IParsable<Jid>.Parse(string s, IFormatProvider provider)
        => Parse(s);

    static bool IParsable<Jid>.TryParse(string s, IFormatProvider provider, out Jid result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(s))
            return false;
        else
            result = Parse(s);

        return result != null;
    }

    public static Jid Parse(string s)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s);

        var ofs = s.IndexOf('@');

        string user = null,
            resource = null,
            server;

        if (ofs != -1)
        {
            user = s[0..ofs];
            s = s[(ofs + 1)..];
        }

        ofs = s.IndexOf('/');

        if (ofs == -1)
            server = s;
        else
        {
            server = s[0..ofs];
            resource = s[(ofs + 1)..];
        }

        return new Jid
        {
            User = user,
            Server = server,
            Resource = resource
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(User))
            sb.Append(User).Append('@');

        sb.Append(Server);

        if (!string.IsNullOrWhiteSpace(Resource))
            sb.Append('/').Append(Resource);

        return sb.ToString();
    }
}
