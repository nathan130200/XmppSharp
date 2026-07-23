using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using XmppSharp.Attributes;

namespace XmppSharp;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class XmppEnum<T> where T : struct, Enum
{
	static readonly IEnumerable<string> s_Names;
	static readonly FrozenDictionary<string, T> s_Members;
	static readonly EqualityComparer<T> s_ValueComparer;

	static XmppEnum()
	{
		var members = from f in typeof(T).GetFields()
					  where f.FieldType == typeof(T)
					  let attr = f.GetCustomAttribute<XmppEnumMemberAttribute>()
					  where attr != null
					  select new
					  {
						  Key = attr.Name,
						  Value = (T)f.GetValue(null)!
					  };

		s_Members = members.ToFrozenDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);

		s_Names = members.Select(x => x.Key);

		s_ValueComparer = EqualityComparer<T>.Default;
	}

	public static IEnumerable<string> GetNames() => s_Names;

	public static IEnumerable<KeyValuePair<string, T>> GetMembers() => s_Members;

	public static T Parse(string value)
	{
		if (s_Members.TryGetValue(value, out var result))
			return result;

		throw new InvalidOperationException($"Cannot parse value '{value}' to enum '{typeof(T).Name}'.");
	}

	public static T? ParseOptional(string? value)
	{
		if (value is not null)
		{
			if (s_Members.TryGetValue(value, out var result))
				return result;
		}

		return null;
	}

	public static T ParseOrDefault(string? value, T defaultValue = default)
	{
		if (value != null)
			return s_Members.GetValueOrDefault(value, defaultValue);

		return defaultValue;
	}

	public static string GetName(T value)
	{
		foreach (var entry in s_Members)
		{
			if (s_ValueComparer.Equals(entry.Value, value))
				return entry.Key;
		}

		throw new InvalidOperationException($"Unknown value '{value}' for enum type '{typeof(T).Name}'.");
	}

	[OverloadResolutionPriority(1)]
	public static string? GetNameOrDefault(T value, string? defaultValue = default)
	{
		foreach (var entry in s_Members)
		{
			if (s_ValueComparer.Equals(entry.Value, value))
				return entry.Key;
		}

		return defaultValue;
	}

	[OverloadResolutionPriority(0)]
	public static string GetNameOrDefault(T value, T defaultValue = default)
	{
		foreach (var entry in s_Members)
		{
			if (s_ValueComparer.Equals(entry.Value, value))
				return entry.Key;
		}

		return GetName(defaultValue);
	}

	public static bool TryParse(string? value, out T result)
	{
		result = default;

		if (!string.IsNullOrWhiteSpace(value))
		{
			if (s_Members.TryGetValue(value, out result))
				return true;
		}

		return false;
	}

	public static bool TryGetName(T value, [NotNullWhen(true)] out string? result)
	{
		result = default;

		foreach (var entry in s_Members)
		{
			if (s_ValueComparer.Equals(entry.Value, value))
			{
				result = entry.Key;
				return true;
			}
		}

		return false;
	}
}