using System.Diagnostics;

namespace XmppSharp;


/// <summary>
/// Immutable version of <see cref="Jid"/>, has no setters instead init-only properties.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct ReadOnlyJid : IEquatable<ReadOnlyJid>
{
    /// <inheritdoc cref="Jid.Empty"/>
    public static ReadOnlyJid Empty => new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Jid _value;

    /// <inheritdoc cref="Jid(string)"/>
    public ReadOnlyJid()
        => _value = new();

    public ReadOnlyJid(Jid other) => _value = new()
    {
        _local = other._local,
        _domain = other._domain,
        _resource = other._resource
    };

    /// <inheritdoc cref="Jid(string)"/>
    public ReadOnlyJid(string jid)
        => _value = new(jid);

    /// <inheritdoc cref="Jid(string, string, string)"/>
    public ReadOnlyJid(string local, string domain, string resource)
        => _value = new(local, domain, resource);

    /// <inheritdoc cref="Jid.Local"/>
    public string? Local
    {
        get => _value.Local;
        init => _value.Local = value;
    }

    /// <inheritdoc cref="Jid.Domain"/>
    public string Domain
    {
        get => _value.Domain;
        init => _value.Domain = value;
    }

    /// <inheritdoc cref="Jid.Resource"/>
    public string? Resource
    {
        get => _value.Resource;
        init => _value.Resource = value;
    }

    /// <inheritdoc cref="Jid.ToString"/>
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

    /// <summary>
    /// Converts between JID and ReadOnlyJID
    /// </summary>
    public static implicit operator Jid(ReadOnlyJid jid)
        => jid._value;

    /// <summary>
    /// Converts between ReadOnlyJID and string.
    /// </summary>
    public static implicit operator string(ReadOnlyJid jid)
        => jid._value.ToString();

    /// <summary>
    /// Converts between ReadOnlyJID and JID.
    /// </summary>
    public static implicit operator ReadOnlyJid(Jid jid)
        => new(jid);
}