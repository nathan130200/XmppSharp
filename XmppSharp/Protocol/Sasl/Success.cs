using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// The <![CDATA[<success>]]> element indicates that the authentication process has completed successfully.
/// </summary>
[Tag("success", Namespaces.Sasl)]
public class Success : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Success"/> class.
    /// </summary>
    public Success() : base("success", Namespaces.Sasl)
    {

    }
}
