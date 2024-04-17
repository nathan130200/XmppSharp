using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

[DebuggerDisplay("{Value,nq}")]
public class Cdata : Node
{
	public Cdata(string value) => this.Value = value;
	public Cdata(Cdata other) => this.Value = other.Value;

	public override Node Clone()
	   => new Cdata(this);

	public override void WriteTo(XmlWriter writer)
		=> writer.WriteCData(this.Value);
}