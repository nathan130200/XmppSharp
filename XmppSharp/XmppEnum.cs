using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

public static class XmppEnum
{
    public static IEnumerable<T> GetMembers<T>() where T : struct, Enum
        => XmppEnum<T>.Members;

    public static IEnumerable<string> GetNames<T>() where T : struct, Enum
        => XmppEnum<T>.NameToMember.Select(x => x.Key);

    public static IEnumerable<string> GetXmlNames<T>() where T : struct, Enum
        => XmppEnum<T>.XmlToMember.Select(x => x.Key);

    public static IReadOnlyDictionary<string, T> GetNameMapping<T>() where T : struct, Enum
        => XmppEnum<T>.NameToMember;

    public static IReadOnlyDictionary<string, T> GetXmlMapping<T>() where T : struct, Enum
        => XmppEnum<T>.XmlToMember;

    public static string? ToXml<T>(T? value) where T : struct, Enum
    {
        if (!value.HasValue)
            return default;

        return ToXml((T)value);
    }

    public static string? ToXml<T>(T value) where T : struct, Enum
    {
        var contract = XmppEnum<T>.EqualityContract;
        var entry = XmppEnum<T>.XmlToMember.FirstOrDefault(x => contract.Equals(x.Value, value));
        return entry.Key;
    }

    public static T FromXml<T>(string? str, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(str))
            return defaultValue;

        if (XmppEnum<T>.XmlToMember.TryGetValue(str, out var result))
            return result;

        return defaultValue;
    }

    public static T? FromXmlOrDefault<T>(string? str) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(str))
            return default;

        if (XmppEnum<T>.XmlToMember.TryGetValue(str, out var result))
            return result;

        return default;
    }
}
public sealed class XmppEnum<T> where T : struct, Enum
{
    public static IEnumerable<T> Members { get; private set; }
    public static IReadOnlyDictionary<string, T> NameToMember { get; private set; }
    public static IReadOnlyDictionary<T, IEnumerable<Attribute>> CustomAttributes { get; private set; }
    public static IReadOnlyDictionary<string, T> XmlToMember { get; private set; }
    public static EqualityComparer<T> EqualityContract { get; set; }

    static XmppEnum()
    {
        if (typeof(T).GetCustomAttribute<XmppEnumAttribute>() == null)
            throw new InvalidOperationException();

        EqualityContract = EqualityComparer<T>.Default;

        Members = Enum.GetValues<T>();

        var fields = from field in typeof(T).GetFields()
                     where field.FieldType == typeof(T)
                     let attribute = field.GetCustomAttribute<XmppMemberAttribute>()
                     select new
                     {
                         field.Name,
                         Value = (T)field.GetValue(null)!,
                         XmlName = attribute?.Value,
                         Attributes = field.GetCustomAttributes().Where(xa => xa is not XmppMemberAttribute)
                     };

        NameToMember = fields.ToDictionary(x => x.Name, x => x.Value);

        XmlToMember = fields.Where(x => x.XmlName != null)
            .ToDictionary(x => x.XmlName, x => x.Value);

        CustomAttributes = fields.ToDictionary(x => x.Value, x => x.Attributes);
    }
}