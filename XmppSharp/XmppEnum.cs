using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

public static class XmppEnum
{
    class Cache<T> where T : struct, Enum
    {
        public static readonly IEnumerable<T> s_Values;
        public static readonly Dictionary<string, T> s_NameToValue;
        public static readonly Dictionary<string, T> s_XmlToValue;
        public static readonly EqualityComparer<T> s_EqualityContract;

        static Cache()
        {
            if (typeof(T).GetCustomAttribute<XmppEnumAttribute>() == null)
                throw new InvalidOperationException();

            s_EqualityContract = EqualityComparer<T>.Default;

            s_Values = Enum.GetValues<T>();

            var fields = from field in typeof(T).GetFields()
                         where field.FieldType == typeof(T)
                         let attribute = field.GetCustomAttribute<XmppMemberAttribute>()
                         select new
                         {
                             field.Name,
                             Value = (T)field.GetValue(null)!,
                             XmlName = attribute?.Value
                         };

            s_NameToValue = fields.ToDictionary(x => x.Name, x => x.Value);

            s_XmlToValue = fields.Where(x => x.XmlName != null)
                .ToDictionary(x => x.XmlName, x => x.Value);
        }
    }

    public static IEnumerable<T> GetValues<T>() where T : struct, Enum
        => Cache<T>.s_Values;

    public static IEnumerable<string> GetNames<T>() where T : struct, Enum
        => Cache<T>.s_NameToValue.Select(x => x.Key);

    public static IEnumerable<string> GetXmlNames<T>() where T : struct, Enum
        => Cache<T>.s_XmlToValue.Select(x => x.Key);

    public static IReadOnlyDictionary<string, T> GetNameMapping<T>() where T : struct, Enum
        => Cache<T>.s_NameToValue;

    public static IReadOnlyDictionary<string, T> GetXmlMapping<T>() where T : struct, Enum
        => Cache<T>.s_XmlToValue;

    public static string? ToXml<T>(T? value) where T : struct, Enum
    {
        if (!value.HasValue)
            return default;

        return ToXml((T)value);
    }

    public static string? ToXml<T>(T value) where T : struct, Enum
    {
        var contract = Cache<T>.s_EqualityContract;
        var entry = Cache<T>.s_XmlToValue.FirstOrDefault(x => contract.Equals(x.Value, value));
        return entry.Key;
    }

    public static T FromXml<T>(string? str, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(str))
            return defaultValue;

        if (Cache<T>.s_XmlToValue.TryGetValue(str, out var result))
            return result;

        return defaultValue;
    }

    public static T? FromXmlOrDefault<T>(string? str) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(str))
            return default;

        if (Cache<T>.s_XmlToValue.TryGetValue(str, out var result))
            return result;

        return default;
    }
}
