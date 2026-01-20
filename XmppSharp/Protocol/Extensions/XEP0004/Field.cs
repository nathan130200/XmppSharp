using System.Globalization;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0004;

[Tag("field", Namespaces.DataForms)]
public class Field : XmppElement
{
	public Field() : base("field", Namespaces.DataForms)
	{

	}

	public Field(FieldType type, string? name = default, string? label = default, string? desc = default) : this()
	{
		Type = type;
		Label = label;
		Name = name;
		Description = desc;
	}

	public FieldType Type
	{
		get => XmppEnum.ParseOrDefault(GetAttribute("type"), FieldType.TextSingle);
		set => SetAttribute("type", value.ToXmlOrDefault(FieldType.TextSingle));
	}

	public string? Name
	{
		get => GetAttribute("var");
		set => SetAttribute("var", value);
	}

	public string? Label
	{
		get => GetAttribute("label");
		set => SetAttribute("label", value);
	}

	public string? Description
	{
		get => GetTag("desc");
		set
		{
			RemoveTag("desc");

			if (value != null)
				SetTag("desc", value: value);
		}
	}

	public bool Required
	{
		get => HasTag("required");
		set
		{
			if (!value)
				RemoveTag("required");
			else
				SetTag("required");
		}
	}

	/// <summary>
	/// Defines the default value for the field (according to the form-processing entity) in a data form.
	/// </summary>
	public string? Value
	{
		get => GetTag("value");
		set
		{
			if (value is null)
				RemoveTag("value");
			else
				SetTag("value", value: value);
		}
	}

	/// <summary>
	/// Defines if this field <see cref="Type"/> may contain more than one <![CDATA[<value/>]]> element.
	/// </summary>
	public bool IsMultiValueSupported
		=> Type is FieldType.ListMulti or FieldType.JidMulti or FieldType.TextMulti or FieldType.Hidden;

	public IEnumerable<string?> Values
	{
		get
		{
			if (!IsMultiValueSupported)
				throw new InvalidOperationException("Field does not support multiple values.");

			return Elements("value").Select(x => x.InnerText!);
		}
		set
		{
			Elements("value")?.Remove();

			if (!IsMultiValueSupported)
				throw new InvalidOperationException("Field does not support multiple values.");

			if (value != null)
			{
				foreach (var item in value)
					SetTag("value", Namespaces.DataForms, item);
			}
		}
	}

	public IEnumerable<Option> Options
	{
		get => Elements<Option>();
		set
		{
			Elements<Option>().Remove();

			if (value?.Any() == true)
			{
				foreach (var item in value)
					AddChild(item);
			}
		}
	}

	public Field AddValue(object? value)
	{
		ArgumentNullException.ThrowIfNull(value);

		if (!IsMultiValueSupported)
		{
			SetTag("value", Namespaces.DataForms, value);
			return this;
		}

		AddChild(new XmppElement("value", Namespaces.DataForms)
		{
			InnerText = Convert.ToString(value, CultureInfo.InvariantCulture)!
		});

		return this;
	}

	public Field AddOption(Option option)
	{
		AddChild(option);
		return this;
	}

	public Field AddOption(Action<Option> builder)
	{
		var option = new Option();
		builder(option);
		AddChild(option);
		return this;
	}
}
