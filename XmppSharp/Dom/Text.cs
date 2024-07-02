using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

public class Text : ContentNode
{
	public Text(string value) => this.Value = value;
	public Text(Text other) => this.Value = other.Value;

	public override Node Clone()
		=> new Text(this);

	public override void WriteTo(XmlWriter writer, in XmlFormatting formatting)
	{
		if (formatting.IncludeTextNodes)
			writer.WriteString(this.Value);
	}
}
