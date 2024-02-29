using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using XmppSharp.Attributes;
using XmppSharp.Collections;

namespace XmppSharp;

public sealed class XmppEnum<TEnum>
     where TEnum : struct, Enum
{
    public static XmppEnum<TEnum> Shared { get; } = new();

    public IBidirectionalMap<string, TEnum> NameMapping { get; }
    public IBidirectionalMap<string, TEnum> XmlNameMapping { get; }

    XmppEnum()
    {
        var baseType = typeof(TEnum);

        var nameMapping = new BidirectionalMap<string, TEnum>();
        var xmlNameMapping = new BidirectionalMap<string, TEnum>();

        foreach (var fieldName in Enum.GetNames<TEnum>())
        {
            var field = baseType.GetField(fieldName);

            var currentValue = (TEnum)field.GetValue(null);
            nameMapping.Add(fieldName, currentValue);

            var attr = field.GetCustomAttribute<XmppEnumMemberAttribute>();

            if (attr != null)
                xmlNameMapping.Add(attr.Value, currentValue);
        }

        NameMapping = nameMapping;
        XmlNameMapping = xmlNameMapping;
    }
}

public static class XmppEnum
{
    public static string ToXml<E>(this E value) where E : struct, Enum
        => XmppEnum<E>.Shared.XmlNameMapping.TryLookup(value, out var result) ? result : default;

    public static string ToName<E>(this E value) where E : struct, Enum
        => XmppEnum<E>.Shared.NameMapping.TryLookup(value, out var result) ? result : default;

    public static E? FromXml<E>(string value) where E : struct, Enum
        => XmppEnum<E>.Shared.XmlNameMapping.TryLookup(value, out var result) ? result : null;

    public static E FromXml<E>(string value, E defaultValue) where E : struct, Enum
        => XmppEnum<E>.Shared.XmlNameMapping.TryLookup(value, out var result) ? result : defaultValue;

    public static E? FromName<E>(string name) where E : struct, Enum
        => XmppEnum<E>.Shared.NameMapping.TryLookup(name, out var result) ? result : null;

    public static IReadOnlyDictionary<string, E> GetXmlMap<E>() where E : struct, Enum
        => XmppEnum<E>.Shared.XmlNameMapping.AsDictionary();

    public static IReadOnlyDictionary<E, string> GetReverseXmlMap<E>() where E : struct, Enum
        => XmppEnum<E>.Shared.XmlNameMapping.AsReverseDictionary();

#pragma warning disable

    static volatile bool _isInitialized;

    [ModuleInitializer]
    internal static void InitializeCache()
    {
        if (_isInitialized)
            return;

        var enumTypes = typeof(XmppEnum).Assembly.GetTypes()
            .Where(t => t.IsEnum)
            .Where(t => t.GetCustomAttributes<XmppEnumAttribute>().Any());

        foreach (var type in enumTypes)
        {
            var typeHandle = typeof(XmppEnum<>)
                .MakeGenericType(type)
                .TypeHandle;

            try
            {
                RuntimeHelpers.RunClassConstructor(typeHandle);
                Debug.WriteLine($"[XmppEnum::InitializeCache] Added enum type '{type}' to cache system.");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[XmppEnum::InitializeCache] Failed to initialize cached enum '{type}': {e}");
            }
        }

        _isInitialized = true;
    }
#pragma warning restore
}
