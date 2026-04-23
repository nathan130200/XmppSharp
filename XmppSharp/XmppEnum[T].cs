using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp;

/// <summary>
/// Provides utility methods for working with enums that are represented as XML values in XMPP.
/// </summary>
/// <typeparam name="T">The type of the enum.</typeparam>
/// <remarks>
/// <para>
/// The enum type <typeparamref name="T"/> must have its members decorated with the <see cref="XmppEnumMemberAttribute"/> to specify the corresponding XML names.
/// </para>
/// <para>
/// This class is intended for internal use and should not be used directly unless there is a very specific reason.
/// </para>
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class XmppEnum<T>
    where T : struct, Enum
{
    /// <summary>
    /// Gets a dictionary that maps XML names to their corresponding enum values of type <typeparamref name="T"/>.
    /// </summary>
    public static FrozenDictionary<string, T> Members { get; }

    static XmppEnum()
    {
        var valueType = typeof(T);

        var fields = from f in valueType.GetFields()
                     where f.FieldType == valueType
                     let attr = f.GetCustomAttribute<XmppEnumMemberAttribute>()
                     where attr != null
                     select new
                     {
                         Key = attr.Name,
                         Value = (T)f.GetValue(null)!
                     };

        Members = fields.ToFrozenDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Tries to get the enum value of type <typeparamref name="T"/> corresponding to the specified XML name.
    /// </summary>
    /// <param name="key">The XML name.</param>
    /// <param name="value">When this method returns, contains the enum value associated with the specified XML name, if the name is found; otherwise, the default value for the enum type.</param>
    /// <returns><c>true</c> if the XML name was found; otherwise, <c>false</c>.</returns>
    public static bool TryGetValue(string? key, out T value)
    {
        value = default;

        if (key is null)
            return false;

        foreach (var it in Members)
        {
            if (it.Key == key)
            {
                value = it.Value;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to get the XML name corresponding to the specified enum value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <param name="key">When this method returns, contains the XML name associated with the specified enum value, if the value is found; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the enum value was found; otherwise, <c>false</c>.</returns>
    public static bool TryGetKey(T value, [NotNullWhen(true)] out string? key)
    {
        key = default;

        foreach (var it in Members)
        {
            if (EqualityComparer<T>.Default.Equals(it.Value, value))
            {
                key = it.Key;
                return true;
            }
        }

        return false;
    }
}