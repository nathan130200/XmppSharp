using System.Security.Cryptography;
using System.Text;
using XmppSharp.Collections;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Utilities
{
    public static byte[] GetBytes(this string str, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(str);

    public static string GetString(this byte[] bytes, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(bytes);

    public static byte[] GetBytes(this XmppNode node)
        => node.ToString()!.GetBytes();

    public static bool IsBareEquals(this Jid? jid, Jid? other)
        => BareJidComparer.Shared.Compare(jid, other) == 0;

    public static bool IsFullEquals(this Jid? jid, Jid? other)
        => FullJidComparer.Shared.Compare(jid, other) == 0;

    static readonly Dictionary<HashAlgorithmName, Func<byte[], byte[]>> s_HashAlgorithms = new();

    static Utilities()
    {
        s_HashAlgorithms[HashAlgorithmName.MD5] = MD5.HashData;
        s_HashAlgorithms[HashAlgorithmName.SHA1] = SHA1.HashData;
        s_HashAlgorithms[HashAlgorithmName.SHA256] = SHA256.HashData;
        s_HashAlgorithms[HashAlgorithmName.SHA384] = SHA384.HashData;
        s_HashAlgorithms[HashAlgorithmName.SHA512] = SHA512.HashData;

#if NET8_0_OR_GREATER
        s_HashAlgorithms[HashAlgorithmName.SHA3_256] = SHA3_256.HashData;
        s_HashAlgorithms[HashAlgorithmName.SHA3_384] = SHA3_384.HashData;
        s_HashAlgorithms[HashAlgorithmName.SHA3_512] = SHA3_512.HashData;
#endif
    }

    static byte[] ComputeHashCore(HashAlgorithmName key, byte[] bytes)
    {
        if (!s_HashAlgorithms.TryGetValue(key, out var hashData))
            throw new InvalidOperationException($"Hash algorithm '{key.Name}' not registered.");

        return hashData(bytes);
    }

    public static byte[] ComputeHash(HashAlgorithmName hashAlgorithm, byte[] data)
        => ComputeHashCore(hashAlgorithm, data);

    public static string ComputeHash(this string str, HashAlgorithmName hashAlgorithm)
        => Convert.ToHexString(ComputeHashCore(hashAlgorithm, str.GetBytes()));
}
