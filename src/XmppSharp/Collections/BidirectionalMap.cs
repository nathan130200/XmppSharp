namespace XmppSharp.Collections;

internal class BidirectionalMap<TKey, TValue> : IBidirectionalMap<TKey, TValue>
{
    readonly struct Pair(TKey key, TValue value)
    {
        public readonly TKey Key = key;
        public readonly TValue Value = value;
    }

    readonly IEqualityComparer<TKey> _keyComparer;
    readonly IEqualityComparer<TValue> _valueComparer;
    readonly List<Pair> _dictionary = [];

    public BidirectionalMap(IEqualityComparer<TKey> keyComparer = default, IEqualityComparer<TValue> valueComparer = default)
    {
        _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
    }

    public IEnumerable<TKey> Keys
        => _dictionary.Select(x => x.Key);

    public IEnumerable<TValue> Values
        => _dictionary.Select(x => x.Value);

#nullable enable

    public TKey? this[TValue item]
        => TryLookup(item, out var result) ? result : default;

    public TValue? this[TKey item]
        => TryLookup(item, out var result) ? result : default;

#nullable restore

    internal void Add(TKey key, TValue second)
        => _dictionary.Add(new Pair(key, second));

    public bool TryLookup(TValue key, out TKey result)
    {
        result = default;

        foreach (var item in _dictionary)
        {
            if (_valueComparer.Equals(item.Value, key))
            {
                result = item.Key;
                return true;
            }
        }

        return false;
    }

    public bool TryLookup(TKey key, out TValue result)
    {
        result = default;

        foreach (var item in _dictionary)
        {
            if (_keyComparer.Equals(item.Key, key))
            {
                result = item.Value;
                return true;
            }
        }
        return false;
    }

    public IReadOnlyDictionary<TKey, TValue> AsDictionary()
        => _dictionary.ToDictionary(x => x.Key, x => x.Value);

    public IReadOnlyDictionary<TValue, TKey> AsReverseDictionary()
        => _dictionary.ToDictionary(x => x.Value, x => x.Key);
}