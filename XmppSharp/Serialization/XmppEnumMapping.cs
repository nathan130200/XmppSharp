using Jabber.Attributes;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jabber.Serialization;

public sealed class XmppEnumMapping<T>
        where T : struct, Enum
{
    public FrozenDictionary<string, T> NameToValue { get; }

    public FrozenDictionary<T, string> ValueToName { get; }

    public static XmppEnumMapping<T> Shared { get; } = new();

    XmppEnumMapping()
    {
        var members = from f in typeof(T).GetFields()
                      where f.FieldType == typeof(T)
                      let attr = f.GetCustomAttribute<XmppEnumMemberAttribute>()
                      where attr != null
                      select new
                      {
                          attr.Name,
                          Value = (T)f.GetValue(null)!
                      };

        NameToValue = members.ToFrozenDictionary(x => x.Name, x => x.Value);

        ValueToName = members.ToFrozenDictionary(x => x.Value, x => x.Name);
    }

    [OverloadResolutionPriority(1)]
    public string? GetNameOrDefault(T value, string? defaultValue = default)
        => ValueToName.TryGetValue(value, out var result) ? result : defaultValue;

    [OverloadResolutionPriority(0)]
    public string? GetNameOrDefault(T value, T fallbackValue = default)
        => ValueToName.TryGetValue(value, out var result) ? result : GetNameOrDefault(fallbackValue, default);

    public string? GetNameOptional(T? value)
    {
        if (value is not T self) return null;

        return GetName(self);
    }

    public string GetName(T? value)
    {
        if (value is not T self)
            throw new ArgumentNullException(nameof(value));

        return ValueToName[self];
    }

    public string GetName(T value) => ValueToName[value];

    [OverloadResolutionPriority(1)]
    public T ParseOrDefault(string? name, T defaultValue = default)
    {
        if (name is null) return defaultValue;

        return NameToValue.TryGetValue(name, out var result) ? result : defaultValue;
    }

    [OverloadResolutionPriority(0)]
    public T ParseOrDefault(string? name, string? defaultValue = default)
    {
        if (name is null) return ParseOrDefault(defaultValue);

        return NameToValue.TryGetValue(name, out var result) ? result : ParseOrDefault(defaultValue!, default);
    }

    public T? ParseOptional(string? name)
    {
        if (name is null) return null;

        return NameToValue.TryGetValue(name, out var result) ? result : null;
    }

    public T Parse([AllowNull] string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return NameToValue[name];
    }
}
