using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("failure", "urn:ietf:params:xml:ns:xmpp-sasl")]
public class Failure : Element
{
	public Failure() : base("failure", Namespaces.Sasl)
	{

	}

	public Failure(FailureCondition condition, string text = default) : this()
	{
		this.Condition = condition;

		if (text != null)
			this.Text = text;
	}

	public FailureCondition Condition
	{
		get
		{
			foreach (var (key, value) in XmppEnum.GetValues<FailureCondition>())
			{
				if (this.HasTag(key, Namespaces.Sasl))
					return value;
			}

			return FailureCondition.Unspecified;
		}
		set
		{
			var other = Condition;

			if (other != FailureCondition.Unspecified)
				this.RemoveTag(other.ToXmppName(), Namespaces.Sasl);

			if (value != FailureCondition.Unspecified)
				this.SetTag(value.ToXmppName(), Namespaces.Sasl);
		}
	}

	public string? Text
	{
		get => this.GetTag("text");
		set
		{
			if (value == null)
				this.RemoveTag("text");
			else
				this.SetTag("text", value: value);
		}
	}
}
