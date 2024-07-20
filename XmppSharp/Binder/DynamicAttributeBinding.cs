using System.Collections;
using System.Dynamic;
using System.Globalization;
using System.Xml.Linq;

namespace XmppSharp.Binder;

internal class DynamicElementBinding : DynamicObject
{
	private readonly Element _parent;

	public DynamicElementBinding(Element parent)
		=> _parent = parent;

	public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
	{
		if (binder.Name == "Add")
		{
			if (args?.Length > 0)
			{
				foreach (var item in args)
				{
					if (item is string s)
						_parent.AddChild(new Text(s));
					else if (item is Node n)
						_parent.AddChild(n);
					else if (item is XElement xel)
						_parent.AddChild(xel.ConvertToElement());
					else
					{
						var str = Convert.ToString(item, CultureInfo.InvariantCulture);

						if (str != null)
							_parent.AddChild(new Text(str));
					}
				}
			}

			result = _parent;

			return true;
		}

		return base.TryInvokeMember(binder, args, out result);
	}

	public override bool TryConvert(ConvertBinder binder, out object? result)
	{
		if (binder.ReturnType == typeof(IEnumerable))
		{
			result = _parent.Nodes();
			return true;
		}

		return base.TryConvert(binder, out result);
	}
}

internal class DynamicAttributeBinding : DynamicObject
{
	private readonly Element _parent;

	public DynamicAttributeBinding(Element parent)
	{
		this._parent = parent;
	}

	public override IEnumerable<string> GetDynamicMemberNames()
		=> _parent.Attributes().Select(x => x.Key);

	public override bool TryGetIndex(GetIndexBinder binder, object[] indices, out object? result)
	{
		result = default;

		if (indices.Length != 1)
			return false;

		return GetAttr(indices[0].ToString()!, out result);
	}

	public override bool TryGetMember(GetMemberBinder binder, out object? result)
	{
		return GetAttr(binder.Name, out result);
	}

	bool GetAttr(string attrName, out object? result)
	{
		var attrRawValue = _parent.GetAttribute(attrName);
		result = new DynamicXmlAttribute(attrName, attrRawValue ?? string.Empty);

		return true;
	}

	public override bool TryDeleteMember(DeleteMemberBinder binder)
	{
		_parent.RemoveAttribute(binder.Name);
		return true;
	}

	public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
	{
		if (indexes.Length != 1)
			return false;

		return true;
	}

	public override bool TrySetMember(SetMemberBinder binder, object? value)
	{
		if (value == null)
			_parent.RemoveAttribute(binder.Name);
		else
			_parent.SetAttribute(binder.Name, value);

		return true;
	}

	public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
	{
		if (indexes.Length != 1)
			return false;

		var attrName = indexes[0].ToString()!;

		if (value == null)
			_parent.RemoveAttribute(attrName);
		else
			_parent.SetAttribute(attrName, value);

		return true;
	}
}