using System.Collections;

namespace XmppSharp.Collections;

public sealed class FullJidComparer : IComparer, IComparer<Jid>
{
    public static FullJidComparer Shared { get; } = new();

    FullJidComparer()
    {

    }

    public static bool AreEquals(Jid? x, Jid? y) => CompreInternal(x, y) == 0;

    static int CompreInternal(Jid? x, Jid? y)
    {
        if (x == null && y == null) return 0;
        else if (x == null) return -1;
        else if (y == null) return 1;

        var result = string.Compare(x.Local, y.Local, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        result = string.Compare(x.Domain, y.Domain, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        return string.Compare(x.Resource, y.Resource, StringComparison.Ordinal);
    }

    public int Compare(object? x, object? y) => CompreInternal(x as Jid, y as Jid);

    public int Compare(Jid? x, Jid? y) => CompreInternal(x, y);
}
