namespace Jabber.Protocol;

[RunStaticCtor]
public readonly struct IqType : IXmppEnum<IqType>
{
    private readonly string _value;
    private readonly static Dictionary<string, IqType> s_cache = [];

    IqType(string value)
    {
        _value = value;
        s_cache.Add(value, this);
    }

    #region IStructEnum Members

    public bool HasValue => _value != null;
    public string Value => _value;

    public static IEnumerable<IqType> Values
        => s_cache.Values;

    public static IqType Parse(string value)
    {
        if (s_cache.TryGetValue(value, out var result))
            return result;

        return default;
    }

    static object IXmppEnum.Parse(string value)
        => Parse(value);

    #endregion

    #region IqType Members

    public override bool Equals(object? obj)
    {
        if (obj is not IqType other)
            return false;

        if (_value == null || other._value == null)
            return false;

        return _value.Equals(other._value);
    }

    public override int GetHashCode() => _value?.GetHashCode() ?? -1;

    public static implicit operator string(IqType obj)
        => obj._value;

    public static bool operator ==(IqType lhs, IqType rhs)
        => lhs.Equals(rhs);

    public static bool operator !=(IqType lhs, IqType rhs)
        => !(lhs == rhs);

    #endregion

    public static IqType Get { get; } = new("get");
    public static IqType Set { get; } = new("set");
    public static IqType Result { get; } = new("result");
    public static IqType Error { get; } = new("error");
}
