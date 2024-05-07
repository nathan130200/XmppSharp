using Expat;
using XmppSharp.Dom;
using XmppSharp.Parsers;

namespace XmppSharp.Test;

[TestClass]
public class ExpatParserTests
{
	[TestMethod]
	public async Task ParseFromBuffer()
	{
		string xml = "<foo xmlns='bar'><baz/></foo>";

		using var parser = new ExpatXmppParser(ExpatEncodingType.Utf8);

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		parser.Write(xml.GetBytes(), true);

		var result = await tcs.Task;

		Console.WriteLine("XML:\n" + result);
	}
}
