using System.Text;

namespace XmppSharp;

public static class Utilities
{
    public static ReadOnlyJid AsReadOnly(this Jid jid)
        => new(jid);

    public static bool TryGetValue<T>(this T? self, out T result) where T : struct
    {
        result = self ?? default;
        return self.HasValue;
    }

    public static string GetString(this byte[] buffer, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(buffer);

    public static byte[] GetBytes(this string s, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(s);

    public static string ToHex(this byte[] bytes, bool lowercase = true)
    {
        var result = Convert.ToHexString(bytes);

        if (!lowercase)
            return result;

        return result.ToLowerInvariant();
    }

    public static byte[] FromHex(this string str)
        => Convert.FromHexString(str);

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> callback)
    {
        Require.NotNull(collection);
        Require.NotNull(callback);

        if (collection.Any())
        {
            foreach (var item in collection)
                callback(item);
        }

        return collection;
    }
}
