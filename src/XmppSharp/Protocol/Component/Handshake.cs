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

        var size = StringHelper.GetMaxByteCount(streamId.Length)
            + StringHelper.GetMaxByteCount(password.Length);

        using var array = new RentedArray<byte>(size, true);

        var pos = StringHelper.GetBytes(streamId, array.Span);

        StringHelper.GetBytes(password, array.Span[pos..]);

        Span<byte> outBuf = stackalloc byte[SHA1.HashSizeInBytes];

        SHA1.HashData(array.Span, outBuf);

        return Convert.ToHexStringLower(outBuf);
    }
}
