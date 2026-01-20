using System.Collections.Concurrent;
using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

public static class XmppEnum
{
	static readonly ConcurrentDictionary<Type, object> s_EnumStates = [];

	internal class State<T> where T : struct, Enum
	{
		public IReadOnlyDictionary<string, T> Members { get; init; }

		public IEnumerable<string> Names => Members.Keys;

		public IEnumerable<T> Values => Members.Values;

		public IEqualityComparer<T> EqualityContract
		{
			get => field ?? EqualityComparer<T>.Default;
			set => field = value;
		}
	}

	internal static State<T> GetState<T>() where T : struct, Enum
	{
		return (State<T>)s_EnumStates.GetOrAdd(typeof(T), _ =>
		{
			var members = from f in typeof(T).GetFields()
						  where f.FieldType == typeof(T)
						  let a = f.GetCustomAttribute<XmppEnumMemberAttribute>()
						  where a != null
						  select new KeyValuePair<string, T>
						  (
							  a.Name,
							  (T)f.GetValue(null)!
						  );

			return new State<T>
			{
				Members = members.ToDictionary()
			};
		});
	}

	public static IEnumerable<KeyValuePair<string, T>> GetMembers<T>() where T : struct, Enum
		=> GetState<T>().Members;

	public static IEnumerable<string> GetNames<T>() where T : struct, Enum
		=> GetState<T>().Names;

	public static T Parse<T>(string? value) where T : struct, Enum
	{
		var state = GetState<T>();

		foreach (var entry in state.Members)
		{
			if (entry.Key == value)
				return entry.Value;
		}

		throw new ArgumentOutOfRangeException(nameof(value));
	}

	public static T? ParseOptional<T>(string? value) where T : struct, Enum
	{
		var state = GetState<T>();

		foreach (var entry in state.Members)
		{
			if (entry.Key == value)
				return entry.Value;
		}

		return null;
	}

	public static T ParseOrDefault<T>(string? value, T defaultValue = default) where T : struct, Enum
	{
		var state = GetState<T>();

		foreach (var entry in state.Members)
		{
			if (entry.Key == value)
				return entry.Value;
		}

		return defaultValue;
	}

	public static string ToXml<T>(this T value) where T : struct, Enum
	{
		var state = GetState<T>();

		foreach (var entry in state.Members)
		{
			if (state.EqualityContract.Equals(entry.Value, value))
				return entry.Key;
		}

		throw new ArgumentOutOfRangeException(nameof(value));
	}

	public static string? ToXmlOrDefault<T>(this T value, string? defaultValue = default) where T : struct, Enum
	{
		var state = GetState<T>();

		foreach (var entry in state.Members)
		{
			if (state.EqualityContract.Equals(entry.Value, value))
				return entry.Key;
		}

		return defaultValue;
	}

	public static string? ToXmlOrDefault<T>(this T value, T defaultValue) where T : struct, Enum
	{
		var state = GetState<T>();

		string? result = null;

		foreach (var entry in state.Members)
		{
			if (state.EqualityContract.Equals(entry.Value, value))
				return entry.Key;

			if (state.EqualityContract.Equals(entry.Value, defaultValue))
				result = entry.Key;
		}

		return result;
	}
}