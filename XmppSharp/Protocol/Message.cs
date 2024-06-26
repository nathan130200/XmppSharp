﻿using XmppSharp.Attributes;
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

	public string Body
	{
		get => this.GetTag("body");
		set
		{
			if (string.IsNullOrEmpty(value))
				this.RemoveTag("body");
			else
				this.SetTag("body", value: value);
		}
	}

	public bool IsXHtml
		=> this.HasTag("body", Namespaces.XHtml);

	public string Subject
	{
		get => this.GetTag("subject");
		set
		{
			if (string.IsNullOrEmpty(value))
				this.RemoveTag("subject");
			else
				this.SetTag("subject", value: value);
		}
	}

	public string Thread
	{
		get => this.GetTag("thread");
		set
		{
			if (string.IsNullOrEmpty(value))
				this.RemoveTag("thread");
			else
				this.SetTag("thread", value: value);
		}
	}
}