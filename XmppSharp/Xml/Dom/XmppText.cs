using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

[DebuggerDisplay("Text: {Value,nq}")]
public class XmppText : XmppNode, IContentNode
{
	public string? Value
	{
		get;
		set;
	}

	public XmppText()
	{

	}

	public XmppText(string? value)
	{
		Value = value;
	}

	public override string ToString()
		=> Value ?? string.Empty;

	public override XmppNode Clone()
		=> new XmppText(Value);

	public override void WriteTo(XmlWriter writer)
		=> writer.WriteString(Value);
}
