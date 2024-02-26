using XmppSharp.Utilities;

namespace XmppSharp.Protocol;

[RunStaticCtor]
public readonly struct IqType : IXmppEnum<IqType>
{
	static readonly Dictionary<string, IqType> s_cache = [];

	public string Value { get; init; }
	public bool HasValue => Value != null;

	IqType(string s)
		=> s_cache[Value = s] = this;

	public static IqType Parse(string value)
		=> s_cache[value];

	public static IEnumerable<IqType> Values
		=> s_cache.Values;

	#region Values

	public static IqType Get { get; } = new("get");
	public static IqType Set { get; } = new("set");
	public static IqType Result { get; } = new("result");
	public static IqType Error { get; } = new("error");

	#endregion

	public override int GetHashCode()
		=> Value?.GetHashCode() ?? -1;

	public override bool Equals(object? obj)
		=> XmppEnumUtil.EqualityComparer(this, obj);

	public static implicit operator string(IqType self)
		=> self.Value;

	public static bool operator ==(IqType lhs, IqType rhs)
		=> lhs.Equals(rhs);

	public static bool operator !=(IqType lhs, IqType rhs)
		=> !(lhs == rhs);
}
