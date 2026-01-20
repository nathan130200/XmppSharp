using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[Tag("error", Namespaces.Stream)]
[Tag("stream:error", Namespaces.Stream)]
public class StreamError : XmppElement
{
	public StreamError() : base("stream:error", Namespaces.Stream)
	{

	}

	public StreamError(StreamErrorCondition condition) : this()
	{
		Condition = condition;
	}

	public StreamError(StreamErrorCondition condition, string? text) : this(condition)
	{
		Text = text;
	}

	public StreamErrorCondition Condition
	{
		get
		{
			foreach (var (name, value) in XmppEnum.GetMembers<StreamErrorCondition>())
			{
				if (HasTag(name, Namespaces.Streams))
					return value;
			}

			return default;
		}
		set
		{
			foreach (var name in XmppEnum.GetNames<StreamErrorCondition>())
				RemoveTag(name, Namespaces.Streams);

			if (Enum.IsDefined(value))
			{
				var name = XmppEnum.ToXml(value)!;
				SetTag(name, Namespaces.Streams);
			}
		}
	}

	public string? Text
	{
		get => GetTag("text", Namespaces.Streams);
		set
		{
			RemoveTag("text", Namespaces.Streams);

			if (value != null)
				SetTag("text", Namespaces.Streams, value);
		}
	}
}
