using System.Diagnostics;
using System.Dynamic;

namespace XmppSharp.Binder;

public class DynamicXmlAttribute : DynamicObject
{
	public string AttributeName { get; }
	public string AttributeValue { get; }

	public DynamicXmlAttribute(string attrName, string attrValue)
	{
		this.AttributeName = attrName;
		this.AttributeValue = attrValue;
	}

	public override bool TryConvert(ConvertBinder binder, out object? result)
	{
		if (binder.ReturnType == typeof(string))
		{
			result = AttributeValue;
			return true;
		}
		else
		{
			result = default;

			var returnType = binder.ReturnType;
			var isNullableType = false;

			{
				var temp = Nullable.GetUnderlyingType(returnType);

				if (temp != null)
				{
					returnType = temp;
					isNullableType = true;
				}
			}

			var func = TryParseHelpers.GetConverter(returnType);

			if (func == null)
			{
				Debug.WriteLine($"[DynamicXmlAttrs::Get] Attribute {AttributeName} parsing failed: {AttributeValue} ({returnType.FullName}).");
				return false;
			}

			result = func(AttributeValue ?? string.Empty);

			if (result is null && !isNullableType)
			{
				Debug.WriteLine($"[DynamicXmlAttrs::Get] Attribute {AttributeName} not found.");

				if (returnType.IsValueType)
					result = Activator.CreateInstance(returnType);

			}

			return true;
		}
	}
}