using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol.Component;

[Tag("handshake", Namespaces.Component)]
public sealed class Handshake() : Element("handshake", Namespaces.Component)
{
	public Handshake(string streamId, string password) : this()
	{
		InnerText = ComputeHash(streamId, password);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool CanAuthenticate(string token) => InnerText == token;

	public static string ComputeHash(string streamId, string password)
	{
		ArgumentNullException.ThrowIfNull(streamId);

		ArgumentNullException.ThrowIfNull(password);

		var bytes = StringHelper.ToBytes(string.Concat(streamId, password));

		Span<byte> buf = stackalloc byte[SHA1.HashSizeInBytes];

		SHA1.HashData(bytes, buf);

		return Convert.ToHexStringLower(buf);
	}
}
