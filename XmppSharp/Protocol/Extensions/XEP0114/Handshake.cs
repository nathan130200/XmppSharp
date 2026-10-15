using System.Security.Cryptography;
using System.Text;

namespace XmppSharp.Protocol.Extensions.XEP0114;

/// <summary>
/// Helper class to create XMPP component handshake.
/// </summary>
public static class Handshake
{
	/// <summary>
	/// Calculates the component's authentication token.
	/// </summary>
	/// <param name="streamId">Stream ID sent by the server.</param>
	/// <param name="secret">Shared secret used by the server to connect.</param>
	/// <returns></returns>
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
