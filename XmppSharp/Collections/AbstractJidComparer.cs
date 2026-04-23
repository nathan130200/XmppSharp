using System.Diagnostics.CodeAnalysis;

namespace XmppSharp.Collections;

internal abstract class AbstractJidComparer : JidComparer, IComparer<Jid>, IEqualityComparer<Jid>
{
    protected abstract IEnumerable<Func<Jid, Jid, int>> Comparators { get; }

    protected static int CompareLocal(Jid x, Jid y)
        => string.Compare(x.Local, y.Local, StringComparison.OrdinalIgnoreCase);

    protected static int CompareDomain(Jid x, Jid y)
        => string.Compare(x.Domain, y.Domain, StringComparison.OrdinalIgnoreCase);

    protected static int CompareResource(Jid x, Jid y)
        => string.Compare(x.Resource, y.Resource, StringComparison.Ordinal);


    public int Compare(Jid? x, Jid? y)
    {
        if (x is null && y is null) return 0;

        if (x is null) return -1;

        if (y is null) return 1;

        int result = 0;

        foreach (var fn in Comparators)
        {
            result = fn(x, y);

            if (result != 0)
                break;
        }

        return result;
    }

    public bool Equals(Jid? x, Jid? y)
    {
        if (x is null && y is null) return true;

        if (x is null || y is null) return false;

        bool result = true;

        foreach (var fn in Comparators)
        {
            if (fn(x, y) != 0)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    protected abstract int GetHashCode([DisallowNull] Jid obj);

    int IEqualityComparer<Jid>.GetHashCode([DisallowNull] Jid obj)
    {
        return GetHashCode(obj);
    }
}
