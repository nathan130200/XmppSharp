using System.Diagnostics.CodeAnalysis;

namespace XmppSharp.Collections;

internal sealed class BareJidComparer : AbstractJidComparer
{
    protected override IEnumerable<Func<Jid, Jid, int>> Comparators { get; } =
    [
        CompareLocal,
        CompareDomain,
    ];

    protected override int GetHashCode([DisallowNull] Jid obj)
    {
        var result = new HashCode();
        result.Add(obj.Local, StringComparer.OrdinalIgnoreCase);
        result.Add(obj.Domain, StringComparer.OrdinalIgnoreCase);
        return result.ToHashCode();
    }
}
