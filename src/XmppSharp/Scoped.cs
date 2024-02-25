namespace XmppSharp;

public readonly struct Scoped<TValue> : IDisposable
{
    private readonly TValue? _value;
    private readonly Action<TValue?>? _callback;

    public Scoped(TValue value, Action<TValue?>? callback)
    {
        _value = value;
        _callback = callback;
    }

    public void Dispose()
        => _callback?.Invoke(_value);

    public TValue? UnsafeValue
        => _value;

    public TValue Value
        => _value!;

    public static implicit operator TValue(Scoped<TValue> self)
        => self._value!;

    public override string ToString()
        => _value?.ToString() ?? string.Empty;

    public static implicit operator string(Scoped<TValue> self)
        => self.ToString()!;
}