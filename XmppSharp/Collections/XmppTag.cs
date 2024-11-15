using System.Diagnostics;

namespace XmppSharp.Collections;

[DebuggerDisplay("Name={TagName}; Namespace={NamespaceURI}")]
public sealed class XmppTag : IEquatable<XmppTag>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string Name { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? Namespace { get; }

    internal XmppTag(string name, string? @namespace)
    {
        Name = name;
        Namespace = @namespace;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine
        (
            Name?.GetHashCode() ?? 0,
            Namespace?.GetHashCode() ?? 0
        );
    }

    public override bool Equals(object? obj)
        => obj is XmppTag other && Equals(other: other);

    public bool Equals(XmppTag? other)
    {
        if (ReferenceEquals(other, null))
            return false;

        return string.Equals(Name, other.Name, StringComparison.Ordinal)
            && string.Equals(Namespace, other.Namespace, StringComparison.InvariantCulture);
    }

    public static bool operator ==(XmppTag? left, XmppTag? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(XmppTag left, XmppTag right) => !(left == right);
}
