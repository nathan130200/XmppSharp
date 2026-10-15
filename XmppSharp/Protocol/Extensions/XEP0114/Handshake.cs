using System.Security.Cryptography;
using System.Text;

namespace XmppSharp.Protocol.Extensions.XEP0114;

public static class Handshake
{
	public static string CreateAuthenticationToken(ReadOnlySpan<char> streamId, ReadOnlySpan<char> secret)
	{
		var size = Encoding.UTF8.GetMaxByteCount(streamId.Length + secret.Length);

		var ofs = 0;

		var input = size <= 128 ? stackalloc byte[128] : new byte[size];

		ofs += Encoding.UTF8.GetBytes(streamId, input[ofs..]);

		ofs += Encoding.UTF8.GetBytes(secret, input[ofs..]);

		Span<byte> hash = stackalloc byte[SHA1.HashSizeInBytes];

		SHA1.HashData(input[..ofs], hash);

		return Convert.ToHexStringLower(hash);
	}
}
