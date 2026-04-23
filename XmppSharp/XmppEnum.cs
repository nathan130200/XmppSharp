using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XmppSharp;

/// <summary>
/// Provides utility methods for working with enums that are represented as XML values in XMPP.
/// </summary>
public static class XmppEnum
{
    // get all "xml" names
    /// <summary>
    /// Gets the XML names of the enum members of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <returns>An enumerable of XML names.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IEnumerable<string> GetNames<T>() where T : struct, Enum
        => XmppEnum<T>.Members.Keys;

    /// <summary>
    /// Gets the enum members of type <typeparamref name="T"/> along with their corresponding XML names.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <returns>An enumerable of key-value pairs where the key is the XML name and the value is the enum member.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IEnumerable<KeyValuePair<string, T>> GetMembers<T>() where T : struct, Enum
        => [.. XmppEnum<T>.Members];

    /// <summary>
    /// Parses the specified XML name into an enum value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="name">The XML name.</param>
    /// <returns>The enum value associated with the specified XML name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the XML name is not found.</exception>
    public static T Parse<T>([MaybeNull] string? name) where T : struct, Enum
    {
        if (!XmppEnum<T>.TryGetValue(name, out var result))
            throw new ArgumentOutOfRangeException(nameof(name));

        return result;
    }

    /// <summary>
    /// Parses the specified XML name into an enum value of type <typeparamref name="T"/> or returns a default value if the XML name is not found.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="name">The XML name.</param>
    /// <param name="defaultValue">The default value to return if the XML name is not found.</param>
    /// <returns>The enum value associated with the specified XML name, or the default value if the name is not found.</returns>
    [OverloadResolutionPriority(1)]
    public static T ParseOrDefault<T>(string? name, T defaultValue = default) where T : struct, Enum
    {
        if (XmppEnum<T>.TryGetValue(name, out var result))
            return result;

        return defaultValue;
    }

    /// <summary>
    /// Parses the specified XML name into an enum value of type <typeparamref name="T"/> or returns null if the XML name is not found.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="name">The XML name.</param>
    /// <returns>The enum value associated with the specified XML name, or null if the name is not found.</returns>
    [OverloadResolutionPriority(0)]
    public static T? ParseOrDefault<T>(string? name) where T : struct, Enum
    {
        if (XmppEnum<T>.TryGetValue(name, out var result))
            return result;

        return null;
    }

    /// <summary>
    /// Converts the specified enum value of type <typeparamref name="T"/> to its corresponding XML name.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>The XML name associated with the specified enum value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the enum value is not found.</exception>
    public static string ToXml<T>(this T value) where T : struct, Enum
    {
        if (!XmppEnum<T>.TryGetKey(value, out var name))
            throw new ArgumentOutOfRangeException(nameof(value));

        return name;
    }

    /// <summary>
    /// Converts the specified enum value of type <typeparamref name="T"/> to its corresponding XML name or returns a default value if the enum value is not found.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <param name="defaultValue">The default value to return if the enum value is not found.</param>
    /// <returns>The XML name associated with the specified enum value, or the default value if the value is not found.</returns>
    [OverloadResolutionPriority(1)]
    public static string? ToXmlOrDefault<T>(this T value, string? defaultValue = default) where T : struct, Enum
    {
        if (XmppEnum<T>.TryGetKey(value, out var name))
            return name;

        return defaultValue;
    }

    /// <summary>
    /// Converts the specified enum value of type <typeparamref name="T"/> to its corresponding XML name or returns the XML name of a fallback enum value if the original enum value is not found.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <param name="fallbackValue">The fallback enum value to use if the original value is not found.</param>
    /// <returns>The XML name associated with the specified enum value, or the XML name of the fallback value if the original value is not found.</returns>
    [OverloadResolutionPriority(0)]
    public static string? ToXmlOrDefault<T>(this T value, T fallbackValue = default) where T : struct, Enum
    {
        if (XmppEnum<T>.TryGetKey(value, out var name))
            return name;

        return ToXml(fallbackValue);
    }
}