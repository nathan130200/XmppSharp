using XmppSharp.Xmpp.Dom;

namespace XmppSharp;

public static class Utilities
{
    public static bool TryGetValue<T>(this T? self, out T result) where T : struct
    {
        result = self ?? default;
        return self.HasValue;
    }

    public static void Remove(this IEnumerable<Element> elements)
    {
        foreach (var element in elements)
            element.Remove();
    }
}
