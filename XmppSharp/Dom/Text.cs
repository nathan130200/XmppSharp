using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

[DebuggerDisplay("{Value,nq}")]
public class Text : Node
{
    public Text(string value) => Value = value;
    public Text(Text other) => Value = other.Value;

    public override Node Clone()
        => new Text(this);

    public override void WriteTo(XmlWriter writer)
        => writer.WriteString(Value);
}
