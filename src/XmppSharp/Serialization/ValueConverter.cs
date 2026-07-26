#if NET6_0

namespace XmppSharp.Serialization;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;

public delegate bool ValueConverterDelegate<T>(ReadOnlySpan<char> s, IFormatProvider? fmt, out T result);

public static class ValueConverter
{
	static readonly ConcurrentDictionary<Type, Delegate> s_Converters = [];

	static ValueConverter()
	{
		// signed types
		RegisterConverter(CreateNumberParser<sbyte>(sbyte.TryParse, NumberStyles.Integer));
		RegisterConverter(CreateNumberParser<short>(short.TryParse, NumberStyles.Integer));
		RegisterConverter(CreateNumberParser<int>(int.TryParse, NumberStyles.Integer));
		RegisterConverter(CreateNumberParser<long>(long.TryParse, NumberStyles.Integer));

		// unsigned types
		RegisterConverter(CreateNumberParser<byte>(byte.TryParse, NumberStyles.Integer));
		RegisterConverter(CreateNumberParser<ushort>(ushort.TryParse, NumberStyles.Integer));
		RegisterConverter(CreateNumberParser<uint>(uint.TryParse, NumberStyles.Integer));
		RegisterConverter(CreateNumberParser<ulong>(ulong.TryParse, NumberStyles.Integer));

		// floating point types
		RegisterConverter(CreateNumberParser<float>(float.TryParse, NumberStyles.Float));
		RegisterConverter(CreateNumberParser<double>(double.TryParse, NumberStyles.Float));
		RegisterConverter(CreateNumberParser<decimal>(decimal.TryParse, NumberStyles.Float));

		// standard .net types

		RegisterConverter<DateTime>(static (s, fmt, out result) =>
		{
			return DateTime.TryParse(s, fmt, DateTimeStyles.None, out result);
		});

		RegisterConverter<DateTimeOffset>(static (s, fmt, out result) =>
		{
			return DateTimeOffset.TryParse(s, fmt, DateTimeStyles.None, out result);
		});

		RegisterConverter<TimeSpan>(static (s, fmt, out result) =>
		{
			return TimeSpan.TryParse(s, fmt, out result);
		});
	}

	delegate bool NumberParser<T>(ReadOnlySpan<char> s, NumberStyles styles, IFormatProvider? fmt, out T result);

	static ValueConverterDelegate<T> CreateNumberParser<T>(NumberParser<T> fn, NumberStyles styles)
	{
		return (s, fmt, out result) =>
		{
			return fn(s, styles, fmt, out result);
		};
	}

	public static void RegisterConverter<T>(ValueConverterDelegate<T> fn)
	{
		var key = typeof(T);

		if (s_Converters.TryAdd(key, fn))
			Trace.WriteLine($"<ValueConverter> Add value converter  for type '{key}' -> {fn}");
		else
		{
			s_Converters[key] = fn;
			Trace.WriteLine($"<ValueConverter> Replace existing value converter for type '{key}' -> {fn}");
		}
	}

	public static bool TryConvert<T>(string? s, IFormatProvider? fmt, out T result)
	{
		result = default!;

		if (!s_Converters.TryGetValue(typeof(T), out var fn))
			return false;

		return ((ValueConverterDelegate<T>)fn)(s, fmt ?? CultureInfo.InvariantCulture, out result);
	}
}

#endif