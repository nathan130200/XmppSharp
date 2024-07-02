using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

[DebuggerDisplay("{Value,nq}")]
public class Comment : ContentNode
{
	public Comment(string value) => this.Value = value;
	public Comment(Comment other) => this.Value = other.Value;

	public override Node Clone()
	   => new Comment(this);

	public override void WriteTo(XmlWriter writer, in XmlFormatting formatting)
	{
		if (formatting.IncludeCommentNodes)
			writer.WriteComment(this.Value);
	}
}
