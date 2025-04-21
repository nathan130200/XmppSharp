using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

public static class XmppEnum
{
    public static IEnumerable<T> GetMembers<T>() where T : struct, Enum
        => State<T>.Members;

    public static IEnumerable<string> GetNames<T>() where T : struct, Enum
        => State<T>.NameToMember.Select(x => x.Key);

    public static IEnumerable<string> GetXmlNames<T>() where T : struct, Enum
        => State<T>.XmlToMember.Select(x => x.Key);

    public static IReadOnlyDictionary<string, T> NameMapping<T>() where T : struct, Enum
        => State<T>.NameToMember;

    public static IReadOnlyDictionary<string, T> XmlMapping<T>() where T : struct, Enum
        => State<T>.XmlToMember;

    public static string? ToXml<T>(T? value) where T : struct, Enum
    {
        if (!value.HasValue)
            return default;

        return ToXml((T)value);
    }

    public static string? ToXml<T>(T value) where T : struct, Enum
    {
        var contract = State<T>.EqualityContract;
        var entry = State<T>.XmlToMember.FirstOrDefault(x => contract.Equals(x.Value, value));
        return entry.Key;
    }

    public static string ToXmlOrThrow<T>(T value) where T : struct, Enum
    {
        var contract = State<T>.EqualityContract;
        var entry = State<T>.XmlToMember.FirstOrDefault(x => contract.Equals(x.Value, value));
        return entry.Key ?? throw new ArgumentException(default, nameof(value));
    }

    public static T FromXmlOrThrow<T>(string? s) where T : struct, Enum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s);

        var result = FromXml<T>(s);

        if (!result.HasValue)
            throw new ArgumentOutOfRangeException(nameof(s));

        return result.Value;
    }

    public static T FromXmlOrDefault<T>(string? str, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(str))
            return defaultValue;

        if (State<T>.XmlToMember.TryGetValue(str, out var result))
            return result;

        return defaultValue;
    }

    public static T? FromXml<T>(string? str) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(str))
            return default;

        if (State<T>.XmlToMember.TryGetValue(str, out var result))
            return result;

        return default;
    }

    public static IEnumerable<Attribute> GetCustomAttributes<T>() where T : struct, Enum
        => State<T>.CustomAttributes;

    public static IEnumerable<Attribute> GetMemberCustomAttributes<T>(T member) where T : struct, Enum
        => State<T>.MemberCustomAttributes[member];

    // ------------------------------------------------------------------------ //

    public static class State<T> where T : struct, Enum
    {
        public static Type EnumType { get; } = typeof(T);
        public static IEnumerable<T> Members { get; private set; }
        public static IReadOnlyDictionary<string, T> NameToMember { get; private set; }
        public static IEnumerable<Attribute> CustomAttributes { get; private set; }
        public static IReadOnlyDictionary<T, IEnumerable<Attribute>> MemberCustomAttributes { get; private set; }
        public static IReadOnlyDictionary<string, T> XmlToMember { get; private set; }
        public static EqualityComparer<T> EqualityContract { get; set; }

        static State()
        {
            if (EnumType.GetCustomAttribute<XmppEnumAttribute>() == null)
                throw new InvalidOperationException($"Type '{EnumType.FullName}' is not valid xmpp enum type.");

            EqualityContract = EqualityComparer<T>.Default;

            Members = Enum.GetValues<T>();
            CustomAttributes = EnumType.GetCustomAttributes();

            var members = from field in typeof(T).GetFields()
                          where field.FieldType == typeof(T)
                          let attributes = field.GetCustomAttributes()
                          let memberAttribute = attributes.OfType<XmppMemberAttribute>().FirstOrDefault()
                          select new
                          {
                              field.Name,
                              Value = (T)field.GetValue(null)!,
                              XmlName = memberAttribute?.Value,
                              Attributes = attributes
                          };

            NameToMember = members.ToDictionary(x => x.Name, x => x.Value);

            XmlToMember = members.Where(x => x.XmlName != null)
                .ToDictionary(x => x.XmlName, x => x.Value);

            MemberCustomAttributes = members.ToDictionary(x => x.Value, x => x.Attributes);
        }
    }
}