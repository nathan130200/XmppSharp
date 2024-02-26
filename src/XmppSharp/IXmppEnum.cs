using System.Numerics;

namespace XmppSharp;

public interface IXmppEnum
{
	string Value { get; init; }
	bool HasValue { get; }
}

public interface IXmppEnum<TEnum> : IXmppEnum,
	IEqualityOperators<TEnum, TEnum, bool>
	where TEnum : struct, IXmppEnum<TEnum>
{
	static abstract IEnumerable<TEnum> Values { get; }
}
