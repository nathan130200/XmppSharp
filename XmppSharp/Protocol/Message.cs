using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol;

public sealed class Message : Stanza
{
	static readonly XmppEnumMapping<MessageType> s_TypeParser
		= XmppEnumMapping<MessageType>.Shared;

	Dictionary<string, string>? _bodies;
	Dictionary<string, string>? _subjects;

	public Message(MessageType type)
	{
		if (!Enum.IsDefined(type))
			type = MessageType.Normal;

		Type = type;
	}

	public Message(Element element) : base(element)
	{
		Type = s_TypeParser.ParseOrDefault(element.Attribute("type"), MessageType.Normal);

		foreach (var body in element.Elements("body"))
		{
			var language = body.GetAttribute("xml:lang") ?? string.Empty;

			_bodies ??= [];

			_bodies[language] = body.Value;
		}

		foreach (var subject in element.Elements("subject"))
		{
			var language = subject.GetAttribute("xml:lang") ?? string.Empty;

			_subjects ??= [];

			_subjects[language] = subject.Value;
		}
	}

	public MessageType Type
	{
		get;
		set;
	}

	public string? GetBody(string language)
	{
		ArgumentNullException.ThrowIfNull(language);

		lock (this)
			return _bodies?.GetValueOrDefault(language);
	}

	public string? GetSubject(string language)
	{
		ArgumentNullException.ThrowIfNull(language);

		lock (this)
			return _subjects?.GetValueOrDefault(language);
	}

	public void SetBody(string language, string? content)
	{
		ArgumentNullException.ThrowIfNull(language);

		lock (this)
		{
			if (content is null)
				_bodies?.Remove(language);
			else
			{
				_bodies ??= [];
				_bodies[language] = content;
			}
		}
	}

	public void SetSubject(string language, string? content)
	{
		ArgumentNullException.ThrowIfNull(language);

		lock (this)
		{
			if (content is null)
				_subjects?.Remove(language);
			else
			{
				_subjects ??= [];
				_subjects[language] = content;
			}
		}
	}

	public string? Body
	{
		get => GetBody(string.Empty);
		set => SetBody(string.Empty, value);
	}

	public string? Subject
	{
		get => GetSubject(string.Empty);
		set => SetSubject(string.Empty, value);
	}

	public string? Thread
	{
		get;
		set;
	}

	public override Element Serialize()
	{
		var result = new Element("message", DefaultNamespace);

		result.SetAttribute("type", s_TypeParser.GetName(Type));
		result.SetAttribute("id", Id);
		result.SetAttribute("from", From);
		result.SetAttribute("to", To);

		lock (this)
		{
			if (_bodies != null)
			{
				foreach (var (language, content) in _bodies)
				{
					var tag = result.C("body");

					if (!string.IsNullOrEmpty(language))
						tag.SetAttribute("xml:lang", language);

					tag.AddChild(new Text(content));
				}
			}

			if (_subjects != null)
			{
				foreach (var (language, content) in _subjects)
				{
					var tag = result.C("subject");

					if (!string.IsNullOrEmpty(language))
						tag.SetAttribute("xml:lang", language);

					tag.AddChild(new Text(content));
				}
			}
		}

		if (!string.IsNullOrEmpty(Thread))
			result.C("thread").Value = Thread;

		foreach (var ext in GetExtensions())
			result.AddChild(ext.Serialize());

		return result;
	}
}