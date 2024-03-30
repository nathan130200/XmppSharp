using System.Diagnostics;
using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

public static class XmppEnum
{
    [StackTraceHidden]
    static void EnsureXmppEnumType<T>()
    {
        _ = typeof(T).GetCustomAttribute<XmppEnumAttribute>()
            ?? throw new InvalidOperationException($"Type '{typeof(T).FullName}' is not valid xmpp enum!");
    }

    public static IEnumerable<string> GetNames<T>() where T : struct, Enum
    {
        EnsureXmppEnumType<T>();
        return XmppEnum<T>.Values.Select(x => x.Key);
    }

    public static IReadOnlyDictionary<string, T> GetValues<T>() where T : struct, Enum
    {
        EnsureXmppEnumType<T>();
        return XmppEnum<T>.Values;
    }

    public static string? ToXmppName<T>(this T value) where T : struct, Enum
    {
        EnsureXmppEnumType<T>();
        return XmppEnum<T>.ToXml(value);
    }

    public static T? Parse<T>(string value) where T : struct, Enum
    {
        EnsureXmppEnumType<T>();
        return XmppEnum<T>.Parse(value);
    }

    public static T ParseOrDefault<T>(string value, T defaultValue) where T : struct, Enum
    {
        EnsureXmppEnumType<T>();
        return XmppEnum<T>.ParseOrDefault(value, defaultValue);
    }

    public static T ParseOrThrow<T>(string value) where T : struct, Enum
    {
        EnsureXmppEnumType<T>();
        return XmppEnum<T>.ParseOrThrow(value);
    }
}

public class XmppEnum<T>
    where T : struct, Enum
{
    public static IReadOnlyDictionary<string, T> Values { get; set; }

    static EqualityComparer<T> EnumComparer { get; }
        = EqualityComparer<T>.Default;

    public static string? ToXml(T value)
    {
        foreach (var (name, self) in Values)
        {
            if (EnumComparer.Equals(self, value))
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

        throw new ArgumentOutOfRangeException(nameof(value), $"This xmpp enum of type {typeof(T).FullName} does not contain a member that matches the value '{value}'");
    }

    static XmppEnum()
    {
        var baseType = typeof(T);
        var attr = baseType.GetCustomAttribute<XmppEnumAttribute>();

        var values = new Dictionary<string, T>();

        foreach (var member in Enum.GetNames<T>())
        {
            var field = baseType.GetField(member);

            var name = field.GetCustomAttribute<XmppMemberAttribute>()?.Name;

            if (name == null)
                continue;

            values[name] = (T)field.GetValue(null);
        }

        Values = values;
    }
}