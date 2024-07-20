#pragma warning disable

using System.Collections.Concurrent;
using System.Globalization;
using System.Numerics;

namespace XmppSharp;

/// <summary>
/// Represents the delegate type of a function that converts string to <typeparamref name="T"/>.
/// </summary>
public delegate object TryParseDelegate(string input);

/// <summary>
/// Collection of common parsers of primitive types.
/// </summary>
public static class TryParseHelpers
{
	public static ConcurrentDictionary<Type, TryParseDelegate> Converters = new()
	{
		[typeof(sbyte)] = CreateSpanParsable<sbyte>(),
		[typeof(byte)] = CreateSpanParsable<byte>(),
		[typeof(short)] = CreateSpanParsable<short>(),
		[typeof(int)] = CreateSpanParsable<int>(),
		[typeof(long)] = CreateSpanParsable<long>(),
		[typeof(ushort)] = CreateSpanParsable<ushort>(),
		[typeof(uint)] = CreateSpanParsable<uint>(),
		[typeof(ulong)] = CreateSpanParsable<ulong>(),
		[typeof(float)] = CreateSpanParsable<float>(),
		[typeof(double)] = CreateSpanParsable<double>(),

		[typeof(Guid)] = CreateParsable<Guid>(),
		[typeof(DateTime)] = CreateParsable<DateTime>(),
		[typeof(DateTimeOffset)] = CreateParsable<DateTimeOffset>(),
		[typeof(TimeSpan)] = CreateParsable<TimeSpan>(),

		[typeof(DateOnly)] = CreateSpanParsable<DateOnly>(),
		[typeof(TimeOnly)] = CreateSpanParsable<TimeOnly>(),

		[typeof(bool)] = TryParseBoolean,
		[typeof(Jid)] = TryParseJid
	};

	public static TryParseDelegate CreateParsable<T>() where T : IParsable<T>
	{
		return new((string s) =>
		{
			if (T.TryParse(s, CultureInfo.InvariantCulture, out var result))
				return result;

			return default;
		});
	}

	public static TryParseDelegate CreateSpanParsable<T>() where T : ISpanParsable<T>
	{
		return new((string s) =>
		{
			if (T.TryParse(s, CultureInfo.InvariantCulture, out var result))
				return result;

			return default;
		});
	}

	public static TryParseDelegate GetConverter(Type type)
		=> Converters.TryGetValue(type, out var func) ? func : null;

	public static TValue ParseOrThrow<TValue>(string value)
	{
		var type = typeof(TValue);
		var func = GetConverter(type);

		if (func == null)
			throw new NotImplementedException($"Value parsing for type {type} was not implemented.");

		var result = func(value);

		if (result is null)
			throw new FormatException("The string is not in the proper format to be parsed.");

		return (TValue)result;
	}

	public static TValue ParseOrDefault<TValue>(string value)
	{
		var type = typeof(TValue);
		var func = GetConverter(type);

		if (func == null)
			throw new NotImplementedException($"Value parsing for type {type} was not implemented.");

		var result = func(value);

		if (result is not null)
			return (TValue)result;

		return default!;
	}

	static object TryParseBoolean(string s)
	{
		bool result = default;

		if (s.Length == 0)
			return null;
		else if (s.Length == 1)
			result = s[0] == '1';
		else
			result = s.Equals("true", StringComparison.InvariantCultureIgnoreCase);

		return bool.TryParse(s, out result) ? result : default;
	}

	static object TryParseJid(string s)
	{
		if (Jid.TryParse(s, out var jid))
			return jid;

		return null;
	}
}

#pragma warning restore