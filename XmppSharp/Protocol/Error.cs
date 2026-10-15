using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol;

/// <summary>
/// A stanza error abstraction.
/// </summary>
[StanzaExtension("error", Namespaces.Client)]
[StanzaExtension("error", Namespaces.Server)]
[StanzaExtension("error", Namespaces.Component)]
public sealed class Error : BaseStanzaExtension
{
	static readonly XmppEnumMapping<ErrorType> s_ErrorTypeParser
		= XmppEnumMapping<ErrorType>.Shared;

	static readonly XmppEnumMapping<ErrorCondition> s_ErrorConditionParser
		= XmppEnumMapping<ErrorCondition>.Shared;

	/// <inheritdoc />
	public Error()
	{
	}

	internal Error(Element element) : base(element)
	{
		Type = s_ErrorTypeParser.Parse(element.Attribute("type"));
		Code = element.Attribute("code");
		CustomCode = element.Attribute("custom_code");

		Text = element.GetTag("text", Namespaces.Stanzas);

		foreach (var tag in element.Elements().Where(x => x.Namespace == Namespaces.Stanzas))
		{
			foreach (var (name, value) in s_ErrorConditionParser.NameToValue)
			{
				if (element.HasTag(name, Namespaces.Stanzas))
				{
					Condition = value;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the stanza error type.
	/// </summary>
	/// <remarks>The type of error can help determine whether or not the error is unrecoverable.</remarks>
	public ErrorType Type
	{
		get;
		set;
	}

	/// <summary>
	/// Gets or sets the stanza error condition.
	/// </summary>
	public ErrorCondition Condition
	{
		get;
		set;
	}

	/// <summary>
	/// Gets or sets a descriptive or diagnostic information about the error.
	/// </summary>
	public string? Text
	{
		get;
		set;
	}

	/// <summary>
	/// Gets or sets the numeric error code.
	/// </summary>
	public int? Code
	{
		get;
		set;
	}

	/// <summary>
	/// Gets or sets the extended numeric error code.
	/// </summary>
	public int? CustomCode
	{
		get;
		set;
	}

	/// <inheritdoc />
	public override Element Serialize()
	{
		var result = new Element("error");
		result.SetAttribute("type", s_ErrorTypeParser.GetName(Type));
		result.SetAttribute("code", Code);
		result.SetAttribute("custom_code", CustomCode);

		if (s_ErrorConditionParser.GetNameOrDefault(Condition) is string condition)
			result.SetTag(condition, Namespaces.Stanzas);

		if (Text is { } text)
			result.SetTag("text", Namespaces.Stanzas, text);

		return result;
	}
}