using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using XmppSharp.Parser;

namespace XmppSharp;

public static partial class Utilities
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

	public static TaskAwaiter<bool> GetAwaiter(this XmppStreamReader parser)
		=> parser.AdvanceAsync().GetAwaiter();

	public static byte[] FromHex(this string str)
		=> Convert.FromHexString(str);

	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> callback)
	{
		Require.NotNull(source);
		Require.NotNull(callback);

		foreach (var item in source)
			callback(item);

		return source;
	}

	public static bool TryGetChild(this Element e, string tagName, string? namespaceURI, out Element? result)
	{
		result = e.Child(tagName, namespaceURI);
		return result != null;
	}

	public static bool TryGetChild(this Element e, string tagName, out Element? result)
	{
		result = e.Child(tagName);
		return result != null;
	}

	public static bool TryMatch(this Regex regex, string input, out Match result)
	{
		result = regex.Match(input);
		return result.Success;
	}

	public static bool TryMatches(this Regex regex, string input, out IEnumerable<Match> result)
	{
		result = regex.Matches(input);
		return result.Any(x => x.Success);
	}
}
