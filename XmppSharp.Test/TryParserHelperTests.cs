namespace XmppSharp.Test;

[TestClass]
public class TryParserHelperTests
{
	[TestMethod]
	public void EnsureAllConvertersInitialized()
	{
		foreach (var (type, parser) in TryParseHelpers.Converters)
		{
			if (parser == null)
			{
				Assert.Fail("Try parser delegate for type {0} is null!", type);
				return;
			}

			Console.WriteLine("Converter for type {0}: {1}", type, parser);
		}
	}

	[TestMethod]
	[DataRow(typeof(uint))]
	[DataRow(typeof(int))]
	[DataRow(typeof(ushort))]
	[DataRow(typeof(short))]
	[DataRow(typeof(long))]
	[DataRow(typeof(ulong))]
	[DataRow(typeof(double))]
	[DataRow(typeof(float))]
	[DataRow(typeof(TimeOnly))]
	[DataRow(typeof(DateOnly))]
	[DataRow(typeof(DateTime))]
	[DataRow(typeof(TimeSpan))]
	[DataRow(typeof(bool))]
	[DataRow(typeof(Jid))]
	[DataRow(typeof(sbyte))]
	[DataRow(typeof(byte))]
	public void GetParserDelegate(Type type)
	{
		var parser = TryParseHelpers.GetConverter(type);
		Assert.IsNotNull(parser);
		Console.WriteLine("GetParserDelegate(): {0} -> {1}", type, parser);
	}

	[TestMethod]
	[DataRow(typeof(uint))]
	[DataRow(typeof(int))]
	[DataRow(typeof(ushort))]
	[DataRow(typeof(short))]
	[DataRow(typeof(long))]
	[DataRow(typeof(ulong))]
	[DataRow(typeof(sbyte))]
	[DataRow(typeof(byte))]
	[DataRow(typeof(double), "2.56")]
	[DataRow(typeof(float), "12.5")]
	[DataRow(typeof(DateOnly), "1/2/3")]
	[DataRow(typeof(TimeOnly), "4:5:6")]
	[DataRow(typeof(bool), "1")]
	[DataRow(typeof(Jid), "foo@bar")]
	[DataRow(typeof(Jid), "foo@bar/baz")]
	[DataRow(typeof(Jid), "foo.component")]
	[DataRow(typeof(DateTime))]
	[DataRow(typeof(TimeSpan))]
	public void TestAllParsers(Type type, string inputData = default)
	{
		var parser = TryParseHelpers.GetConverter(type);
		Assert.IsNotNull(parser);
		Console.WriteLine("TestAllParsers(): {0} -> {1}", type, parser(inputData ?? "0"));
	}
}
