using System.Diagnostics.CodeAnalysis;

namespace XmppSharp.Collections;

public sealed class XmppName : IEquatable<XmppName>
{
    public string LocalName { get; private set; }
    public string? Prefix { get; private set; }
    public bool HasPrefix => Prefix != null;

    ~XmppName()
    {
        LocalName = null!;
        Prefix = null;
    }

    public XmppName(string? str)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(str);

        var ofs = str!.IndexOf(':');

        if (ofs > 0)
            Prefix = str[0..ofs];

        LocalName = str[(ofs + 1)..];

        ThrowHelper.ThrowIfNullOrWhiteSpace(LocalName);
    }

    public override string ToString()
    {
        if (!HasPrefix)
            return LocalName;

        return string.Concat(Prefix, ':', LocalName);
    }
    public override int GetHashCode() => HashCode.Combine
    (
        LocalName?.GetHashCode() ?? 0,
        Prefix?.GetHashCode() ?? 0
    );

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is XmppName other && Equals(other);

    public bool Equals(XmppName? other)
    {
        if (other is null)
            return false;

        var result = string.Compare(LocalName, other.LocalName, StringComparison.Ordinal); // xml rules

        if (result != 0)
            return false;

        return string.Compare(Prefix, other.Prefix, StringComparison.Ordinal) == 0; // xml rules
    }

    public static implicit operator XmppName(string? str) => new(str);
    public static implicit operator string(XmppName name) => name.ToString();

    public static bool operator ==(XmppName lhs, XmppName rhs)
    {
        if (lhs is null || rhs is null)
            return lhs is null && rhs is null;

        return lhs.Equals(rhs);
    }

    public static bool operator !=(XmppName lhs, XmppName rhs) => !(lhs == rhs);
}
