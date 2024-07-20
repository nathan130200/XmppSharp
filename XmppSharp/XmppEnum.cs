using System.Diagnostics;
using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

public static class XmppEnum
{
	[StackTraceHidden]
	static void ThrowIfNotXmppEnum<T>()
	{
		if (!IsValidType<T>())
			throw new XmppEnumException($"Type '{typeof(T).FullName}' is not valid xmpp enum!");
	}

	/// <summary>
	/// Determines whether the given type is a valid XMPP enum type.
	/// </summary>
	public static bool IsValidType<T>()
		=> typeof(T).GetCustomAttributes<XmppEnumAttribute>().Any();

	public static bool IsDefined<T>(T value) where T : struct, Enum
	{
		ThrowIfNotXmppEnum<T>();

		foreach (var (_, other) in XmppEnum<T>.Values)
		{
			if (XmppEnum<T>.Comparer.Equals(other, value))
				return true;
		}

		return false;
	}

	/// <summary>
	/// Gets all XMPP names mapped to the given type.
	/// </summary>
	/// <typeparam name="T">Enum type annotated with <see cref="XmppEnumAttribute"/></typeparam>
	public static IEnumerable<string> GetNames<T>() where T : struct, Enum
	{
		ThrowIfNotXmppEnum<T>();
		return XmppEnum<T>.Values.Select(x => x.Key);
	}

	/// <summary>
	/// Gets the mapping of all names and values mapped to the given type.
	/// </summary>
	/// <typeparam name="T">Enum type annotated with <see cref="XmppEnumAttribute"/></typeparam>
	public static IReadOnlyDictionary<string, T> GetValues<T>() where T : struct, Enum
	{
		ThrowIfNotXmppEnum<T>();
		return XmppEnum<T>.Values;
	}

	/// <summary>
	/// Converts the value of the given enum to the XMPP name.
	/// </summary>
	/// <typeparam name="T">Enum type annotated with <see cref="XmppEnumAttribute"/></typeparam>
	/// <param name="value">Enum value that will be mapped.</param>
	public static string? ToXmppName<T>(this T value) where T : struct, Enum
	{
		ThrowIfNotXmppEnum<T>();

		if (!IsDefined(value))
			return null;

		return XmppEnum<T>.ToXml(value);
	}

	/// <summary>
	/// Parses the given string into the target XMPP enum, or <see langword="null"/> if no match is found.
	/// </summary>
	/// <typeparam name="T">Enum type annotated with <see cref="XmppEnumAttribute"/></typeparam>
	/// <param name="value">String that will be mapped.</param>
	public static T? Parse<T>(string? value) where T : struct, Enum
	{
		ThrowIfNotXmppEnum<T>();

		if (string.IsNullOrWhiteSpace(value))
			return default;

		return XmppEnum<T>.Parse(value);
	}

	/// <summary>
	/// Parses the given string into the target XMPP enum, or returns the provided fallback value if none is match.
	/// </summary>
	/// <typeparam name="T">Enum type annotated with <see cref="XmppEnumAttribute"/></typeparam>
	/// <param name="value">String that will be mapped.</param>
	/// <param name="defaultValue">Fallback value in case no match is found.</param>
	public static T ParseOrDefault<T>(string? value, T defaultValue) where T : struct, Enum
	{
		ThrowIfNotXmppEnum<T>();

		if (string.IsNullOrWhiteSpace(value))
			return defaultValue;

		return XmppEnum<T>.ParseOrDefault(value, defaultValue);
	}

	public static bool TryParse<T>(string? value, out T result) where T : struct, Enum
	{
		var temp = Parse<T>(value);
		result = temp!.Value;
		return temp.HasValue;
	}

	/// <summary>
	/// Parses the string provided in the destination XMPP enum, or throws an exception if none is match
	/// </summary>
	/// <typeparam name="T">Enum type annotated with <see cref="XmppEnumAttribute"/></typeparam>
	/// <param name="value">String that will be mapped.</param>
	public static T ParseOrThrow<T>(string value) where T : struct, Enum
	{
		ThrowIfNotXmppEnum<T>();
		return XmppEnum<T>.ParseOrThrow(value);
	}
}

internal class XmppEnum<T>
	where T : struct, Enum
{
	public static IReadOnlyDictionary<string, T> Values { get; set; }
	public static EqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;

	public static string? ToXml(T value)
	{
		foreach (var (name, self) in Values)
		{
			if (Comparer.Equals(self, value))
				return name;
		}

		return default;
	}

	public static T? Parse(string value)
	{
		foreach (var (name, self) in Values)
		{
			if (name == value)
				return self;
		}

		return default;
	}

	public static T ParseOrDefault(string value, T defaultValue)
	{
		foreach (var (name, self) in Values)
		{
			if (name == value)
				return self;
		}

		return defaultValue;
	}

	public static T ParseOrThrow(string value)
	{
		foreach (var (name, self) in Values)
		{
			if (name == value)
				return self;
		}

		throw new XmppEnumException($"This xmpp enum of type {typeof(T).FullName} does not contain a member that matches the value '{value}'");
	}

	static XmppEnum()
	{
		var baseType = typeof(T);

		var values = new Dictionary<string, T>();

		foreach (var member in Enum.GetNames<T>())
		{
			var field = baseType.GetField(member)!;

			var name = field.GetCustomAttribute<XmppMemberAttribute>()?.Name;

			if (name == null)
				continue;

			values[name] = (T)field.GetValue(null)!;
		}

		Values = values;
	}
}