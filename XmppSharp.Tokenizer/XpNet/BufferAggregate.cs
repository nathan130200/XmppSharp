namespace XmppSharp.XpNet;

public sealed class BufferAggregate : IDisposable
{
    private Node? _head, _tail;

    internal class Node
    {
        public byte[]? buf;
        public Node? next;
    }

    private MemoryStream? _stream;

    public BufferAggregate()
    {
        _stream = new();
    }

    public void Dispose()
    {
        Clear(0);
        _stream?.Dispose();
        _stream = null;
    }

    public void Write(byte[] buf)
    {
        _stream!.Write(buf, 0, buf.Length);

        if (_tail == null)
        {
            _head = _tail = new Node();
            _head.buf = buf;
        }
        else
        {
            var newNode = new Node
            {
                buf = buf
            };

            _tail.next = newNode;
            _tail = newNode;
        }
    }

    public void Clear(int offset)
    {
        int temp = 0;
        int saveBytes = -1;

        Node it;

        for (it = _head; it != null; it = it.next)
        {
            if (temp + it.buf.Length <= offset)
            {
                if (temp + it.buf.Length == offset)
                {
                    it = it.next;
                    break;
                }
                temp += it.buf.Length;
            }
            else
            {
                saveBytes = temp + it.buf.Length - offset;
                break;
            }
        }

        _head = it;

        if (_head == null)
            _tail = null;

        if (saveBytes > 0)
        {
            var result = new byte[saveBytes];
            Buffer.BlockCopy(_head.buf, _head.buf.Length - saveBytes, result, 0, saveBytes);
            _head.buf = result;
        }

        _stream.SetLength(0);

        for (it = _head; it != null; it = it.next)
            _stream.Write(it.buf, 0, it.buf.Length);
    }

    public byte[] GetBuffer()
    {
        return _stream.ToArray();
    }
}