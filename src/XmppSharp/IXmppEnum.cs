namespace Jabber;

public interface IXmppEnum
{
    bool HasValue { get; }
    string Value { get; }
    abstract static object Parse(string value);
}

public interface IXmppEnum<T> : IXmppEnum
    where T : notnull
{
    abstract static new T Parse(string value);
    abstract static IEnumerable<T> Values { get; }
}