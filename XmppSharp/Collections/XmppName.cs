using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace XmppSharp.Collections;

[DebuggerDisplay("{_debugString,nq}")]
public sealed class XmppName : IEquatable<XmppName>
{
    public string LocalName { get; }
    public string? Prefix { get; }
    public bool HasPrefix => Prefix != null;

    readonly string _debugString;

    public XmppName(XmppName other)
    {
        Throw.IfNull(other);

        LocalName = other.LocalName;
        Prefix = other.Prefix;
        _debugString = other._debugString;
    }

    public XmppName(string? str)
    {
        Throw.IfStringNullOrWhiteSpace(str);

        var ofs = str!.IndexOf(':');

        if (ofs > 0)
            Prefix = XmlConvert.EncodeLocalName(str[0..ofs]);

        LocalName = XmlConvert.EncodeLocalName(str[(ofs + 1)..]);

        Throw.IfStringNullOrWhiteSpace(LocalName);

        _debugString = XmlConvert.EncodeName(str);
    }

    public override string ToString()
        => _debugString;

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

        return string.Equals(_debugString, other._debugString, StringComparison.Ordinal);
    }

    [return: NotNullIfNotNull(nameof(str))]
    public static implicit operator XmppName?(string? str) => str == null ? null : new(str);

    [return: NotNullIfNotNull(nameof(name))]
    public static implicit operator string?(XmppName? name) => name?.ToString();

    public static bool operator ==(XmppName lhs, XmppName rhs)
    {
        if (lhs is null || rhs is null)
            return lhs is null && rhs is null;

        return lhs.Equals(rhs);
    }

    public static bool operator !=(XmppName lhs, XmppName rhs) => !(lhs == rhs);
}
