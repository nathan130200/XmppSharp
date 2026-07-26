#if NET6_0

namespace XmppSharp.Serialization;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

public delegate bool ValueConverter<T>(ReadOnlySpan<char> s, IFormatProvider? fmt, out T result);

public delegate bool NumberParser<T>(ReadOnlySpan<char> s, NumberStyles styles, IFormatProvider? fmt, out T result);

public static class ValueConverter
{
	static readonly ConcurrentDictionary<Type, Delegate> s_Converters = [];

	static ValueConverter()
	{
		var defaultNumberStyle = NumberStyles.Number;

		// signed types
		RegisterConverter(CreateNumberParser<sbyte>(sbyte.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<short>(short.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<int>(int.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<long>(long.TryParse, defaultNumberStyle));

		// unsigned types
		RegisterConverter(CreateNumberParser<byte>(byte.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<ushort>(ushort.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<uint>(uint.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<ulong>(ulong.TryParse, defaultNumberStyle));

		// floating point types
		RegisterConverter(CreateNumberParser<float>(float.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<double>(double.TryParse, defaultNumberStyle));
		RegisterConverter(CreateNumberParser<decimal>(decimal.TryParse, defaultNumberStyle));

		// other .net types

		RegisterConverter<bool>(static (s, fmt, out result) =>
		{
			result = default;

			if (s.Length == 1)
			{
				result = s[0] == '1';
				return true;
			}
			else
			{
				if (s.Equals(bool.TrueString, StringComparison.Ordinal))
					result = true;
				else if (s.Equals(bool.FalseString, StringComparison.Ordinal))
					result = false;
				else
				{
					if (!BigInteger.TryParse(s, NumberStyles.Any, fmt, out var temp)) // all attempts to parse boolean!
						return bool.TryParse(s, out result);

					result = temp >= BigInteger.One;
				}
			}

			return true;
		});

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

		// custom types

		RegisterConverter<Jid>(static (s, fmt, out result) =>
		{
			result = default!;

			if (s != null)
				result = new Jid(s.ToString());

			return result != null;
		});
	}

	public static ValueConverter<T> CreateNumberParser<T>(NumberParser<T> fn, NumberStyles styles)
	{
		ArgumentNullException.ThrowIfNull(fn);

		return (s, fmt, out result) =>
		{
			try
			{
				return fn(s, styles, fmt, out result);
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"<ValueConverter> Uncaught exception while parsing value '{s}' to type '{typeof(T)}': {ex}");

				result = default!;

				return false;
			}
		};
	}

	public static bool TryGetConverter<T>([NotNullWhen(true)] out ValueConverter<T>? result)
	{
		result = default;

		if (s_Converters.TryGetValue(typeof(T), out var fn))
			result = (ValueConverter<T>)fn;

		return result != null;
	}

	public static ValueConverter<T> GetConverter<T>()
	{
		var key = typeof(T);

		if (s_Converters.TryGetValue(key, out var fn))
			return (ValueConverter<T>)fn;

		throw new InvalidOperationException($"Unknown value converter for type '{key}'.");
	}

	public static void RegisterConverter<T>(ValueConverter<T> fn)
	{
		var key = typeof(T);

		if (s_Converters.TryAdd(key, fn))
			Trace.WriteLine($"<ValueConverter> Add value converter for type '{key}'.");
		else
		{
			Trace.WriteLine($"<ValueConverter> Replace existing value converter for type '{key}'.");
			s_Converters[key] = fn;
		}
	}

	public static bool TryConvert<T>(string? s, IFormatProvider? fmt, out T result)
	{
		result = default!;

		if (!s_Converters.TryGetValue(typeof(T), out var fn))
			return false;

		return ((ValueConverter<T>)fn)(s, fmt ?? CultureInfo.InvariantCulture, out result);
	}
}

#endif