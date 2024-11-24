namespace XmppSharp.Parser;

/// <summary>
/// Interface to abstract xmpp parser APIs based on dynamic byte streams.
/// </summary>
public interface IXmppChunkedParser
{
    /// <summary>
    /// Tells the underlying parser to add bytes for XML processing.
    /// </summary>
    /// <param name="buffer">Buffer containing possible XML tokens to be processed.</param>
    /// <param name="length">Amount of bytes to process from <paramref name="buffer"/>.</param>
    void Write(byte[] buffer, int length);
}
