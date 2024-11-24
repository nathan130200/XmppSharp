using System.Xml;

namespace XmppSharp.Parser;

/// <summary>
/// Interface to abstract xmpp parser APIs based on <see cref="XmlReader" /> or that need a constant stream.
/// </summary>
public interface IXmppStreamProcessor
{
    /// <summary>
    /// Tells underlying xml reader to proceed with reading XML chars and processing XML tokens.
    /// </summary>
    /// <returns>
    /// Returns <see langword="true"/> if the parser was able to move forward and processed something; <see langword="false" /> if you were unable to read it or if an error occurred.
    /// <para>
    /// Note that this function will block until it has enough data for the reader to process the XML.
    /// </para>
    /// </returns>
    bool Advance();
}
