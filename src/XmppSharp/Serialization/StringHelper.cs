using System.Text;

namespace XmppSharp.Serialization;

public static class StringHelper
{
	// initialize UTF-8 without BOM here
	static readonly UTF8Encoding s_UTF8_NotBOM = new(false);

	public static int GetMaxByteCount(int length)
		=> s_UTF8_NotBOM.GetMaxByteCount(length);

	public static int GetBytes(ReadOnlySpan<char> input, Span<byte> output)
		=> s_UTF8_NotBOM.GetBytes(input, output);

	public static byte[] ToBytes(string str)
		=> s_UTF8_NotBOM.GetBytes(str);

	public static string ToString(byte[] buf)
		=> s_UTF8_NotBOM.GetString(buf);

	public static string ToHex(string s)
	{
		var maxBytes = GetMaxByteCount(s.Length);

		using var array = new RentedArray<byte>(maxBytes);

		var written = GetBytes(s, array.Span);

		return Convert.ToHexString(array.Span[..written]);
	}

	public static string ToHexLower(string s)
	{
		var maxBytes = GetMaxByteCount(s.Length);

		using var array = new RentedArray<byte>(maxBytes);

		var written = GetBytes(s, array.Span);

		return Convert.ToHexStringLower(array.Span[..written]);
	}
}
