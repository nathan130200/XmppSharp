using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("failure", Namespaces.Sasl)]
public sealed class Failure() : Element("failure", Namespaces.Sasl)
{
	public Failure(FailureCondition condition, string? text = default) : this()
	{
		Condition = condition;
		Text = text;
	}

	public FailureCondition Condition
	{
		get
		{
			foreach (var (key, value) in XmppEnum<FailureCondition>.GetMembers())
			{
				if (HasTag(key, Namespaces.Sasl))
					return value;
			}

			return default;
		}

		set
		{
			foreach (var name in XmppEnum<FailureCondition>.GetNames())
				RemoveTag(name, Namespaces.Sasl);

			if (Enum.IsDefined(value))
				SetTag(XmppEnum<FailureCondition>.GetName(value), Namespaces.Sasl);
		}
	}

	public string? Text
	{
		get => GetTag("text", Namespaces.Sasl);
		set
		{
			if (value is null)
				RemoveTag("text", Namespaces.Sasl);
			else
				SetTag("text", Namespaces.Sasl, value);
		}
	}
}