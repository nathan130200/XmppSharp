using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;

namespace XmppSharp.Dynamic;

/// <summary>
/// Represents the class for dynamic handling the XML attributes.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class DynamicAttributeDictionary : DynamicObject
{
	private Element _element;

	public DynamicAttributeDictionary(Element element) : base()
	{
		this._element = element;
	}

	public override bool TryGetMember(GetMemberBinder binder, out object? result)
	{
		var rawValue = this._element.GetAttribute(binder.Name);
		result = new DynamicAttributeValue(binder.Name, rawValue);
		return true;
	}

	public override bool TrySetMember(SetMemberBinder binder, object? value)
	{
		if (value is DynamicAttributeValue self)
			this._element.SetAttribute(binder.Name, self.Value);
		else
			this._element.SetAttribute(binder.Name, value);

		return true;
	}
}

/// <summary>
/// Represents the class that contains a dynamic converter of the XML attribute value.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class DynamicAttributeValue : DynamicObject
{
	public DynamicAttributeValue(string name, string value)
	{
		this.Name = name;
		this.Value = value;
	}

	public string Name { get; }
	public string Value { get; }

	public override bool TryConvert(ConvertBinder binder, out object? result)
	{
		result = default;

		try
		{
			if (!string.IsNullOrWhiteSpace(Value))
			{
				var func = TryParseHelpers.GetConverter(binder.ReturnType);

				if (func != null)
				{
					var temp = func(Value);

					if (temp != null)
						result = temp;
				}
			}

			if (result == null)
			{
				result = binder.ReturnType.IsValueType
					? Activator.CreateInstance(binder.ReturnType)
					: default;
			}

			return true;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}

		return false;
	}
}