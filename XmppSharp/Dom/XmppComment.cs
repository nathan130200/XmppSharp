using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

[DebuggerDisplay("Comment: {Value,nq}")]
public class XmppComment : XmppNode
{
    public string? Value
    {
        get;
        set;
    }

    public XmppComment()
    {

    }

    public XmppComment(string? value)
    {
        Value = value;
    }

    public override string ToString()
        => $"<!--{Value}-->";

    public override XmppNode Clone()
        => new XmppComment(Value);

    public override void WriteTo(XmlWriter writer)
        => writer.WriteComment(Value);
}
