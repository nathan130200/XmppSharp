using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

// ---------------------------------------------------- //

[DebuggerDisplay("Cdata: {Value,nq}")]
public class XmppCdata : XmppNode, IContentNode
{
	public string? Value
	{
		get;
		set;
	}

	public XmppCdata()
	{

	}

	public XmppCdata(string? value)
	{
		Value = value;
	}

	public override string ToString()
		=> $"<![CDATA[{Value}]]>";

	public override XmppNode Clone()
		=> new XmppCdata(Value);

	public override void WriteTo(XmlWriter writer)
		=> writer.WriteCData(Value);
}