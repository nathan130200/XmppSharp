namespace XmppSharp.Collections;

public interface IBidirectionalMap<TKey, TValue>
{
    TValue? this[TKey key] { get; }
    TKey? this[TValue key] { get; }

    bool TryLookup(TValue key, out TKey? result);
    bool TryLookup(TKey key, out TValue? result);

    IEnumerable<TKey> Keys { get; }
    IEnumerable<TValue> Values { get; }

    IReadOnlyDictionary<TKey, TValue> AsDictionary();
    IReadOnlyDictionary<TValue, TKey> AsReverseDictionary();
}