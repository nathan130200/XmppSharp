using System.Diagnostics;
using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

public static class XmppEnum
{
    [StackTraceHidden]
    static void CheckEnum<T>(out string ns)
    {
        var attr = typeof(T).GetCustomAttribute<XmppEnumAttribute>();
        ns = attr?.Namespace;

        if (attr == null)
            throw new InvalidOperationException("Type is not valid xmpp enum!");
    }

    public static string GetXmlName<T>(this T value) where T : struct, Enum
        => ToXml(value);

    public static IEnumerable<string> GetNames<T>() where T : struct, Enum
    {
        CheckEnum<T>(out _);
        return XmppEnum<T>.Values.Select(x => x.Key);
    }

    public static IReadOnlyDictionary<string, T> GetValues<T>() where T : struct, Enum
    {
        CheckEnum<T>(out _);
        return XmppEnum<T>.Values;
    }

    public static string? ToXml<T>(T value) where T : struct, Enum
    {
        CheckEnum<T>(out _);
        return XmppEnum<T>.ToXml(value);
    }

    public static T? FromXml<T>(string value) where T : struct, Enum
    {
        CheckEnum<T>(out _);
        return XmppEnum<T>.FromXml(value);
    }

    public static T FromXml<T>(string value, T fallbackValue) where T : struct, Enum
    {
        CheckEnum<T>(out _);
        return XmppEnum<T>.FromXml(value, fallbackValue);
    }

    public static string GetNamespace<T>() where T : struct, Enum
    {
        CheckEnum<T>(out var ns);
        return ns;
    }
}

public class XmppEnum<T>
    where T : struct, Enum
{
    public static string Namespace { get; }
    public static IReadOnlyDictionary<string, T> Values { get; set; }

    static readonly EqualityComparer<T> s_EqualityComparer = EqualityComparer<T>.Default;

    public static string? ToXml(T value)
    {
        foreach (var (name, self) in Values)
        {
            if (s_EqualityComparer.Equals(self, value))
                return name;
        }

        return default;
    }

    public static T? FromXml(string value)
    {
        foreach (var (name, self) in Values)
        {
            if (name == value)
                return self;
        }

        return default;
    }

    public static T FromXml(string value, T fallbackValue)
    {
        foreach (var (name, self) in Values)
        {
            if (name == value)
                return self;
        }

        return fallbackValue;
    }

    [Conditional("DEBUG")]
    static void LogWarning(string msg)
        => Debug.WriteLine(msg);

    static XmppEnum()
    {
        var baseType = typeof(T);
        var attr = baseType.GetCustomAttribute<XmppEnumAttribute>();
        Namespace = attr.Namespace;

        var values = new Dictionary<string, T>();

        foreach (var member in Enum.GetNames<T>())
        {
            var field = baseType.GetField(member);

            var name = field.GetCustomAttribute<XmppEnumMemberAttribute>()?.Name;

            if (name == null)
                continue;

            values[name] = (T)field.GetValue(null);
        }

        Values = values;
    }
}