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
        if (elements.Any())
        {
            foreach (var element in elements)
                element.Remove();
        }
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> callback)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(callback);

        if (enumerable.Any())
        {
            foreach (var item in enumerable)
                callback(item);
        }

        return enumerable;
    }
}
