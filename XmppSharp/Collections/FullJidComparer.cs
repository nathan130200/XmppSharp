using System.Diagnostics.CodeAnalysis;

namespace XmppSharp.Collections;

internal sealed class FullJidComparer : AbstractJidComparer
{
    protected override IEnumerable<Func<Jid, Jid, int>> Comparators { get; } =
    [
        CompareLocal,
        CompareDomain,
        CompareResource,
    ];

    protected override int GetHashCode([DisallowNull] Jid obj) => obj.GetHashCode();
}