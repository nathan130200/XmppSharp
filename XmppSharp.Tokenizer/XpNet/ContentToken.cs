using System.Runtime.CompilerServices;

namespace XmppSharp.XpNet;

public class ContentToken : Token
{
    const int DefaultCapacity = 8;

    private int _count;
    private int _capacity;

    private int[] _nameStart = new int[DefaultCapacity];
    private int[] _nameEnd = new int[DefaultCapacity];
    private int[] _valueStart = new int[DefaultCapacity];
    private int[] _valueEnd = new int[DefaultCapacity];
    private bool[] _normalized = new bool[DefaultCapacity];

    public int GetAttributeSpecifiedCount()
        => _count;

    public int GetAttributeNameStart(int i)
    {
        ThrowOutOfRange(i);
        return _nameStart[i];
    }

    public int GetAttributeNameEnd(int i)
    {
        ThrowOutOfRange(i);
        return _nameEnd[i];
    }

    public int GetAttributeValueStart(int i)
    {
        ThrowOutOfRange(i);
        return _valueStart[i];
    }

    public int GetAttributeValueEnd(int i)
    {
        ThrowOutOfRange(i);
        return _valueEnd[i];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowOutOfRange(int index, [CallerArgumentExpression(nameof(index))] string expression = default)
    {
        if (index >= _count)
            throw new ArgumentOutOfRangeException(expression, index, null);
    }

    public bool IsAttributeNormalized(int i)
    {
        ThrowOutOfRange(i);
        return _normalized[i];
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Clear()
    {
        _count = 0;

        _nameStart = new int[DefaultCapacity];
        _nameEnd = new int[DefaultCapacity];
        _valueStart = new int[DefaultCapacity];
        _valueEnd = new int[DefaultCapacity];
        _normalized = new bool[DefaultCapacity];

        _capacity = DefaultCapacity;
    }

    static void GrowArray<T>(ref T[] source)
    {
        var result = new T[source.Length + DefaultCapacity];
        Array.ConstrainedCopy(source, 0, result, 0, source.Length);
        source = result;
    }

    void GrowIfNeeded()
    {
        if (_count == _capacity)
        {
            GrowArray(ref _nameStart);
            GrowArray(ref _nameEnd);
            GrowArray(ref _valueStart);
            GrowArray(ref _valueEnd);
            GrowArray(ref _normalized);
            _capacity += DefaultCapacity;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void AppendAttribute(int nameStart, int nameEnd, int valueStart, int valueEnd, bool isNormalized)
    {
        GrowIfNeeded();

        _nameStart[_count] = nameStart;
        _nameEnd[_count] = nameEnd;
        _valueStart[_count] = valueStart;
        _valueEnd[_count] = valueEnd;
        _normalized[_count] = isNormalized;
        _count++;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void CheckAttributeUniqueness(byte[] buf)
    {
        for (int i = 1; i < _count; i++)
        {
            int len = _nameEnd[i] - _nameStart[i];

            for (int j = 0; j < i; j++)
            {
                if (_nameEnd[j] - _nameStart[j] == len)
                {
                    int size = len;
                    int s1 = _nameStart[i];
                    int s2 = _nameStart[j];

                    do
                    {
                        if (--size < 0)
                            throw new InvalidTokenException(_nameStart[i], InvalidTokenType.DuplicatedAttribute);

                    } while (buf[s1++] == buf[s2++]);
                }
            }
        }
    }
}
