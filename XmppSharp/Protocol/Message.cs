using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol;

/// <summary>
/// An abstraction of a message stanza.
/// </summary>
public sealed class Message : Stanza
{
	static readonly XmppEnumMapping<MessageType> s_TypeParser
		= XmppEnumMapping<MessageType>.Shared;

	Dictionary<string, string>? _bodies;
	Dictionary<string, string>? _subjects;

	/// <summary>
	/// Initialize a new instance of <see cref="Message"/> class.
	/// </summary>
	/// <param name="type">The type of message.</param>
	public Message(MessageType type)
	{
		if (!Enum.IsDefined(type))
			type = MessageType.Normal;

		Type = type;
	}

	/// <summary>
	/// Initialize a new instance of <see cref="Message"/> class from given element.
	/// </summary>
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

	/// <summary>
	/// The message type.
	/// </summary>
	public MessageType Type
	{
		get;
		set;
	}

	/// <summary>
	/// Gets the message body.
	/// </summary>
	/// <param name="language">The language identifier for the desired language code.</param>
	/// <returns>The message body.</returns>
	/// <remarks>The <paramref name="language"/> parameter can be empty string to get message body with default language (without a <c>xml:lang</c> attribute)</remarks>
	public string? GetBody(string language)
	{
		ArgumentNullException.ThrowIfNull(language);

		lock (this)
			return _bodies?.GetValueOrDefault(language);
	}


	/// <summary>
	/// Gets the message subject.
	/// </summary>
	/// <param name="language">The language identifier code (or empty string for default language).</param>
	public string? GetSubject(string language)
	{
		ArgumentNullException.ThrowIfNull(language);

		lock (this)
			return _subjects?.GetValueOrDefault(language);
	}

	/// <summary>
	/// Sets the message body.
	/// </summary>
	/// <param name="language">The language identifier code (or empty string for default language).</param>
	/// <param name="content">The message body text for the given language.</param>
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

	/// <summary>
	/// Sets the message subject.
	/// </summary>
	/// <param name="language">The language identifier code (or empty string for default language).</param>
	/// <param name="content">The message subject for the given language.</param>
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

	/// <summary>
	/// Gets or sets message body with default language.
	/// </summary>
	public string? Body
	{
		get => GetBody(string.Empty);
		set => SetBody(string.Empty, value);
	}

	/// <summary>
	/// Gets or sets message subject with default language.
	/// </summary>
	public string? Subject
	{
		get => GetSubject(string.Empty);
		set => SetSubject(string.Empty, value);
	}

	/// <summary>
	/// The thread ID of a message stanza.
	/// </summary>
	/// <remarks>Empty for non-message stanzas.</remarks>
	public string? Thread
	{
		get;
		set;
	}

	/// <inheritdoc />
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