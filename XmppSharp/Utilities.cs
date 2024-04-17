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

	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> callback)
	{
		Require.NotNull(source);
		Require.NotNull(callback);

		if (source is IList<T> list)
		{
			foreach (var item in list)
				callback(item);
		}
		else
		{
			foreach (var item in source)
				callback(item);
		}

		return source;
	}

	public static bool TryGetChild(this Element e, string localName, string? namespaceURI, out Element result)
	{
		result = e.Child(localName, namespaceURI);
		return result != null;
	}
}
