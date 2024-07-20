using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("message", Namespaces.Client)]
[XmppTag("message", Namespaces.Server)]
[XmppTag("message", Namespaces.Accept)]
[XmppTag("message", Namespaces.Connect)]
public class Message : Stanza
{
	public Message() : base("message", Namespaces.Client)
	{

	}

	public Message(MessageType type) : this()
		=> this.Type = type;

	public new MessageType Type
	{
		get => XmppEnum.ParseOrDefault(base.Type, MessageType.Normal);
		set
		{
			if (value == MessageType.Normal)
				base.Type = null;
			else
				base.Type = value.ToXmppName();
		}
	}

	public string? Body
	{
		get => this.GetTag("body");
		set
		{
			this.RemoveTag("body");

			if (value != null)
				this.SetTag("body", value: value);
		}
	}

	public string? Subject
	{
		get => this.GetTag("subject");
		set
		{
			this.RemoveTag("subject");

			if (value != null)
				this.SetTag("subject", value: value);
		}
	}

	public string? Thread
	{
		get => this.GetTag("thread");
		set
		{
			this.RemoveTag("thread");

			if (value != null)
				this.SetTag("thread", value: value);
		}
	}
}