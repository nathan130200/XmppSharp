using System.Xml;

namespace XmppSharp.Protocol;

[RunStaticCtor]
public readonly struct StanzaErrorType : IXmppEnum<StanzaErrorType>
{
    private readonly string _value;
    private readonly static Dictionary<string, StanzaErrorType> s_cache = [];

    StanzaErrorType(string value)
    {
        _value = value;
        s_cache.Add(value, this);
    }

    #region IStructEnum Members

    public bool HasValue => _value != null;
    public string Value => _value;

    public static StanzaErrorType Parse(string value)
    {
        if (s_cache.TryGetValue(value, out var result))
            return result;

        return default;
    }

    public static IEnumerable<StanzaErrorType> Values
        => s_cache.Values;

    static object IXmppEnum.Parse(string value)
        => Parse(value);

    #endregion

    #region ErrorType Struct Members

    public override int GetHashCode() => _value?.GetHashCode() ?? -1;

    public override bool Equals(object? obj)
    {
        if (obj is not StanzaErrorType other)
            return false;

        if (!HasValue || !other.HasValue)
            return false;

        return _value == other._value;
    }

    public static implicit operator string(StanzaErrorType v)
        => v._value;

    public static bool operator ==(StanzaErrorType lhs, StanzaErrorType rhs)
        => lhs.Equals(rhs);

    public static bool operator !=(StanzaErrorType lhs, StanzaErrorType rhs)
        => !(lhs == rhs);

    public XmlElement CreateElement(string ns, StanzaErrorCondition condition, string? message = default, XmlDocument? document = default)
    {
        var element = Xml.Element("error", ns, document);
        document ??= element.OwnerDocument;

        element.SetAttribute("type", _value);
        element.AppendChild(condition.CreateElement(document));

        if (!string.IsNullOrEmpty(message))
            element.C("text", Namespace.Stanzas).T(message);

        return element;
    }

    #endregion

    /// <summary>
    /// Retry after providing credentials
    /// </summary>
    public static StanzaErrorType Auth { get; } = new("auth");

    /// <summary>
    /// Do not retry (the error cannot be remedied)
    /// </summary>
    public static StanzaErrorType Cancel { get; } = new("cancel");

    /// <summary>
    /// Proceed (the condition was only a warning)
    /// </summary>
    public static StanzaErrorType Continue { get; } = new("continue");

    /// <summary>
    /// Retry after changing the data sent
    /// </summary>
    public static StanzaErrorType Modify { get; } = new("modify");

    /// <summary>
    /// Retry after waiting (the error is temporary)
    /// </summary>
    public static StanzaErrorType Wait { get; } = new("wait");
}
