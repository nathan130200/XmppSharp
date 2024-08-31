#pragma warning disable

using System.Collections.Concurrent;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

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
	static TryParseHelpers()
	{

	}

#pragma warning disable

	[ModuleInitializer]
	internal static void Init()
	{

	}

#pragma warning restore

#if NET6_0

	public static ConcurrentDictionary<Type, TryParseDelegate> Converters { get; } = new()
	{
		[typeof(sbyte)] = CreateNumberParser<sbyte>(),
		[typeof(byte)] = CreateNumberParser<byte>(),
		[typeof(short)] = CreateNumberParser<short>(),
		[typeof(int)] = CreateNumberParser<int>(),
		[typeof(long)] = CreateNumberParser<long>(),
		[typeof(ushort)] = CreateNumberParser<ushort>(),
		[typeof(uint)] = CreateNumberParser<uint>(),
		[typeof(ulong)] = CreateNumberParser<ulong>(),
		[typeof(float)] = CreateNumberParser<float>(NumberStyles.Float),
		[typeof(double)] = CreateNumberParser<double>(NumberStyles.Float),

		[typeof(Guid)] = CreateDefaultParser<Guid>(),

		[typeof(DateTime)] = CreateDateOrTimeParser<DateTime>(),
		[typeof(DateTimeOffset)] = CreateDateOrTimeParser<DateTimeOffset>(),
		[typeof(DateOnly)] = CreateDateOrTimeParser<DateOnly>(),
		[typeof(TimeOnly)] = CreateDateOrTimeParser<TimeOnly>(),

		[typeof(bool)] = TryParseBoolean,
		[typeof(Jid)] = TryParseJid,
		[typeof(TimeSpan)] = TryParseTimeSpan
	};

	static TryParseDelegate CreateDateOrTimeParser<T>()
	{
		var xMethod = typeof(T).GetMethod("TryParse", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
			new Type[]
			{
				typeof(string),
				typeof(IFormatProvider),
				typeof(DateTimeStyles),
				typeof(T).MakeByRefType()
			});

		if (xMethod == null)
			return null;

		return new(souce =>
		{
			var args = new object[4];
			args[0] = souce;
			args[1] = CultureInfo.InvariantCulture;
			args[2] = DateTimeStyles.None;

			var result = (bool)xMethod.Invoke(null, args);

			if (result)
				return (T)args[3];

			return null;
		});
	}

	static TryParseDelegate CreateDefaultParser<T>()
	{
		var xMethod = typeof(T).GetMethod("TryParse", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
			new Type[]
			{
				typeof(string),
				typeof(T).MakeByRefType()
			});

		if (xMethod == null)
			return null;

		return new(souce =>
		{
			var args = new object[2];
			args[0] = souce;
			var result = (bool)xMethod.Invoke(null, args);

			if (result)
				return (T)args[1];

			return null;
		});
	}

	static MethodInfo FindTryParseMethod(Type type, params Type[] paramTypes)
	{
		return type.GetMethods()
			.Where(x => x.Name == "TryParse")
			.FirstOrDefault(m =>
			{
				var isMatch = true;

				var methodParams = m.GetParameters();

				for (int i = 0; i < Math.Min(paramTypes.Length, methodParams.Length); i++)
				{
					if (methodParams[i].ParameterType != paramTypes[i])
					{
						isMatch = false;
						break;
					}
				}

				return m.IsStatic && isMatch;
			});
	}

	static TryParseDelegate CreateNumberParser<T>(NumberStyles hint = NumberStyles.Integer)
	{
		var xMethod = FindTryParseMethod(typeof(T),
			new Type[]
			{
				typeof(string),
				typeof(NumberStyles),
				typeof(IFormatProvider),
				typeof(T).MakeByRefType()
			});

		if (xMethod == null)
			return null;

		return new(source =>
		{
			var args = new object[4];
			args[0] = source;
			args[1] = hint;
			args[2] = CultureInfo.InvariantCulture;

			var result = (bool)xMethod.Invoke(null, args);

			if (result)
				return (T)args[3];

			return null;
		});
	}

#else

	public static ConcurrentDictionary<Type, TryParseDelegate> Converters { get; } = new()
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

#endif

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

#if NET6_0

	static object TryParseTimeSpan(string s)
	{
		if (TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out var result))
			return result;

		return null;
	}

#endif
}

#pragma warning restore