using System.Text;
using System.Xml.Linq;

namespace XmppSharp;

public static class Utils
{
	public static bool TryUnwrap<S>(this S? value, out S result) where S : struct
	{
		result = value ?? default;
		return value.HasValue;
	}

	public static byte[] GetBytes(this string s, Encoding? encoding = default)
		=> (encoding ?? Encoding.UTF8).GetBytes(s);

	public static byte[] GetBytes(this XNode node, Encoding? encoding = default)
		=> node.ToString(false).GetBytes(encoding);

	public static string ToString(this XNode node, bool indent)
	{
		var options = SaveOptions.OmitDuplicateNamespaces;

		if (!indent)
			options |= SaveOptions.DisableFormatting;

		return node.ToString(options);
	}
}
