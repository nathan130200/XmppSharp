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
		NumberStyles defaultIntegerFormat = NumberStyles.Integer,
			defaultFloatFormat = NumberStyles.Float;

		// signed types
		RegisterConverter(CreateNumberParser<sbyte>(sbyte.TryParse, defaultIntegerFormat));
		RegisterConverter(CreateNumberParser<short>(short.TryParse, defaultIntegerFormat));
		RegisterConverter(CreateNumberParser<int>(int.TryParse, defaultIntegerFormat));
		RegisterConverter(CreateNumberParser<long>(long.TryParse, defaultIntegerFormat));

		// unsigned types
		RegisterConverter(CreateNumberParser<byte>(byte.TryParse, defaultIntegerFormat));
		RegisterConverter(CreateNumberParser<ushort>(ushort.TryParse, defaultIntegerFormat));
		RegisterConverter(CreateNumberParser<uint>(uint.TryParse, defaultIntegerFormat));
		RegisterConverter(CreateNumberParser<ulong>(ulong.TryParse, defaultIntegerFormat));

		// floating point types
		RegisterConverter(CreateNumberParser<float>(float.TryParse, defaultFloatFormat));
		RegisterConverter(CreateNumberParser<double>(double.TryParse, defaultFloatFormat));
		RegisterConverter(CreateNumberParser<decimal>(decimal.TryParse, defaultFloatFormat));

		// other .net types

		RegisterConverter<bool>(static (s, fmt, out result) =>
		{
			result = default;

			if (s.Length == 1)
			{
				if (char.IsDigit(s[0]))
				{
					var isDecimalTrue = (s[0] - '0') > 0;
					result = isDecimalTrue;
					return true;
				}

				// don't consider letters as valid boolean value

				return false;
			}
			else
			{
				if (s.Equals(bool.TrueString, StringComparison.Ordinal))
					result = true;

				else if (s.Equals(bool.FalseString, StringComparison.Ordinal))
					result = false;

				else
				{
					if (BigInteger.TryParse(s, NumberStyles.Integer, fmt, out var asInt))
					{
						result = asInt > BigInteger.Zero;
						return true;
					}

					if (double.TryParse(s, NumberStyles.Float, fmt, out var asDouble))
					{
						if (double.IsNaN(asDouble) || double.IsInfinity(asDouble))
							goto fallback;

						result = asDouble > 0d;

						return true;
					}

					fallback:

					return bool.TryParse(s, out result);
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