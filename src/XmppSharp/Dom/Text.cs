using System.Globalization;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

public class Text : Node
{
	public Text()
	{

	}

	public Text(object? value, IFormatProvider? fmt = default)
	{
		Value = Convert.ToString(value, fmt ?? CultureInfo.InvariantCulture);
	}

	public string? Value
	{
		get;
		set;
	}

	public override Node Clone() => new Text(Value);

	public override void WriteTo(XmlWriter writer)
	{
		if (Value != null)
			writer.WriteString(Value);
	}
}
