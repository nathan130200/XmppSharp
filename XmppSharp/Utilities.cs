using System.Text;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Utilities
{
	extension(byte[] bytes)
	{
		public string GetString(Encoding? encoding = null)
			=> (encoding ?? Encoding.UTF8).GetString(bytes);

		public string GetHex() => Convert.ToHexString(bytes);
		public string GetHexLower() => Convert.ToHexStringLower(bytes);
	}

	extension(string s)
	{
		public byte[] GetBytes(Encoding? encoding = default)
			=> (encoding ?? Encoding.UTF8).GetBytes(s);

		public string GetHex()
			=> Convert.ToHexString(s.GetBytes());

		public string GetHexLower()
			=> Convert.ToHexStringLower(s.GetBytes());
	}

	public static void Remove<T>(this IEnumerable<T> source) where T : XmppNode
	{
		ArgumentNullException.ThrowIfNull(source);

		foreach (var node in source)
			node.Remove();
	}

	public static bool? GetAttributeBool(this XmppElement e, string name)
	{
		if (e.GetAttribute(name) is string s)
		{
			if (s == "1") return true;

			if (int.TryParse(s, out var i))
				return i > 0;

			return bool.TryParse(s, out var b) && b;
		}

		return null;
	}

	public static bool GetAttributeBool(this XmppElement e, string name, bool defaultValue)
		=> e.GetAttributeBool(name) ?? defaultValue;

	public static void SetAttributeBool(this XmppElement e, string name, bool? value)
	{
		if (value is null) e.RemoveAttribute(name);
		else e.SetAttribute(name, ((bool)value) ? 1 : 0);
	}
}
