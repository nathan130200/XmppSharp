using System.Diagnostics;

namespace XmppSharp;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct ReadOnlyJid : IEquatable<ReadOnlyJid>
{
    public static ReadOnlyJid Empty => new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Jid _value;

    public ReadOnlyJid()
        => _value = new();

    public ReadOnlyJid(Jid other) => _value = new()
    {
        _local = other._local,
        _domain = other._domain,
        _resource = other._resource
    };

    public ReadOnlyJid(string jid)
        => _value = new(jid);

    public ReadOnlyJid(string local, string domain, string resource)
        => _value = new(local, domain, resource);

    public string? Local
    {
        get => _value.Local;
        init => _value.Local = value;
    }

    public string Domain
    {
        get => _value.Domain;
        init => _value.Domain = value;
    }

    public string? Resource
    {
        get => _value.Resource;
        init => _value.Resource = value;
    }

    public override string ToString()
        => _value.ToString();

    public override int GetHashCode()
        => _value.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is not ReadOnlyJid other)
            return false;

        return Equals(other);
    }

    public bool Equals(ReadOnlyJid other)
    {
        if (_value is null)
            return other._value is null;

        return _value.Equals(other._value);
    }

    public static bool operator ==(ReadOnlyJid lhs, ReadOnlyJid rhs)
        => lhs.Equals(rhs);

    public static bool operator !=(ReadOnlyJid lhs, ReadOnlyJid rhs)
        => !(lhs == rhs);

    public static implicit operator Jid(ReadOnlyJid jid)
        => jid._value;

    public static implicit operator ReadOnlyJid(Jid jid)
        => new(jid);
}

public static class JidHelpers
{
    public static ReadOnlyJid AsReadOnly(this Jid jid)
        => new(jid);
}