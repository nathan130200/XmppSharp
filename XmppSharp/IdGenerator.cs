using System.Buffers.Binary;

namespace XmppSharp;

public abstract class IdGenerator
{
    public static IdGenerator Random { get; } = new RandomIdGenerator();
    public static IdGenerator Guid { get; } = new GuidIdGenerator();
    public static IdGenerator Sequential { get; } = new SequentialIdGenerator();
    public static IdGenerator Timestamp { get; } = new TimestampIdGenerator();

    public abstract string Generate();

    class GuidIdGenerator : IdGenerator
    {
        public override string Generate()
            => System.Guid.NewGuid().ToString("d");
    }

    class SequentialIdGenerator : IdGenerator
    {
        static volatile uint _value = 1U;

        public override string Generate()
            => _value++.ToString("x8");
    }

    class TimestampIdGenerator : IdGenerator
    {
        public override string Generate()
        {
            lock (this)
            {
                return DateTimeOffset.UtcNow
                    .ToUnixTimeMilliseconds()
                    .ToString();
            }
        }
    }

    class RandomIdGenerator : IdGenerator
    {
        public override string Generate()
        {
            lock (this)
            {
                var buf = new byte[4];
                System.Random.Shared.NextBytes(buf);
                return BinaryPrimitives.ReadUInt32BigEndian(buf).ToString();
            }
        }
    }
}