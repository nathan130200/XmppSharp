using System.Text;
using XmppSharp.Collections;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Helpers
{
    public static byte[] GetBytes(this string str, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(str);

    public static string GetString(this byte[] bytes, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(bytes);

    public static byte[] GetBytes(this Node node)
        => node.ToString()!.GetBytes();

    public static bool IsBareEquals(this Jid? jid, Jid? other)
        => BareJidComparer.Shared.Compare(jid, other) == 0;

    public static bool IsFullEquals(this Jid? jid, Jid? other)
        => FullJidComparer.Shared.Compare(jid, other) == 0;
}
