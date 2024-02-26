using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using XmppSharp.Utilities;

namespace XmppSharp.Protocol;

[RunStaticCtor]
public readonly struct StanzaErrorType : IXmppEnum<StanzaErrorType>
{
	private static readonly Dictionary<string, StanzaErrorType> s_cache = [];

	public string Value { get; init; }
	public bool HasValue => Value != null;

	StanzaErrorType(string value)
		=> s_cache.Add(Value = value, this);

	public static StanzaErrorType Parse(string value)
		=> s_cache[value];

	public static IEnumerable<StanzaErrorType> Values
		=> s_cache.Values;

	public static implicit operator string(StanzaErrorType self)
		=> self.Value;

	public override int GetHashCode()
		=> Value?.GetHashCode() ?? -1;

	public override bool Equals([NotNullWhen(true)] object? obj)
		=> XmppEnumUtil.EqualityComparer(this, obj);

	public static bool operator ==(StanzaErrorType lhs, StanzaErrorType rhs)
		=> lhs.Equals(rhs);

	public static bool operator !=(StanzaErrorType lhs, StanzaErrorType rhs)
		=> !(lhs == rhs);

	public XElement CreateElement(string xmlns, StanzaErrorCondition? condition = default, string? message = default)
	{
		var element = Xml.Element("error", xmlns);
		element.SetAttributeValue("type", Value);

		if (condition.TryUnwrap(out var v))
			element.C(v.CreateElement());

		if (!string.IsNullOrEmpty(message))
			element.C("text", Namespaces.Stanzas).Text(message);

		return element;
	}

	/// <summary>
	/// Retry after providing credentials
	/// </summary>
	public static StanzaErrorType Auth { get; } = new("auth");

	/// <summary>
	/// Do not retry (the error cannot be remedied)
	/// </summary>
	public static StanzaErrorType Cancel { get; } = new("cancel");

	/// <summary>
	/// Proceed (the condition was only a warning)
	/// </summary>
	public static StanzaErrorType Continue { get; } = new("continue");

	/// <summary>
	/// Retry after changing the data sent
	/// </summary>
	public static StanzaErrorType Modify { get; } = new("modify");

	/// <summary>
	/// Retry after waiting (the error is temporary)
	/// </summary>
	public static StanzaErrorType Wait { get; } = new("wait");
}
