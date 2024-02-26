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

	public static string ToXml(this XNode node, bool indent = false)
	{
		var options = SaveOptions.OmitDuplicateNamespaces;

		if (!indent)
			options |= SaveOptions.DisableFormatting;

		return node.ToString(options);
	}
}