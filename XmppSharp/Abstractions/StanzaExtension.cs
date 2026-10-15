using System.Runtime.CompilerServices;
using XmppSharp.Dom;

namespace XmppSharp.Abstractions;

public abstract class StanzaExtension : IXmppSerializable
{
    protected StanzaExtension()
    {

    }

    protected StanzaExtension(Element element)
    {
        ArgumentNullException.ThrowIfNull(element);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public abstract Element Serialize();
}
