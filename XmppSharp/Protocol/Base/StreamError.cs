using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:error", "http://etherx.jabber.org/streams")]
public class StreamError : Element
{
	public StreamError() : base("stream:error", Namespaces.Stream)
	{

	}

	public StreamError(StreamErrorCondition condition, string text = default) : this()
	{
		this.Condition = condition;

		if (text != null)
			this.Text = text;
	}

	public StreamErrorCondition Condition
	{
		get
		{
			foreach (var (name, value) in XmppEnum.GetValues<StreamErrorCondition>())
			{
				if (this.HasTag(name, Namespaces.Streams))
					return value;
			}

			return StreamErrorCondition.UndefinedCondition;
		}
		set
		{
			this.RemoveTag(this.Condition.ToXmppName(), Namespaces.Streams);

			if (XmppEnum.IsDefined(value))
				this.SetTag(value.ToXmppName(), Namespaces.Streams);
		}
	}

	public string? Text
	{
		get => this.GetTag("text", Namespaces.Streams);
		set
		{
			if (value == null)
				this.RemoveTag("text", Namespaces.Streams);
			else
				this.SetTag("text", Namespaces.Streams, value);
		}
	}
}