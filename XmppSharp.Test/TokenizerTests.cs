using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using NuGet.Frameworks;
using XmppSharp.Parser;
using XmppSharp.XpNet;

namespace XmppSharp.Test;

[TestClass]
public class TokenizerTests
{
	static readonly byte[] StartTagData = "<foo xmlns='bar' xmlns:foo='bar'>"u8.ToArray();
	static readonly byte[] EndTagData = "</foo>"u8.ToArray();
	static readonly byte[] ElementData = StartTagData.Concat(EndTagData).ToArray();

	// Source: https://onlinetools.com/random/generate-random-xml
	static readonly byte[] FullTestData = Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<root>
  <paint species=""surface"">1528515908</paint>
  <choice>-1264968518.9362125</choice>
  <caught belong=""nearby"">
    <signal proper=""follow"">-1433047885.2501898
      <![CDATA[enter children solar leader friend]]>
    </signal>
    <![CDATA[baseball]]>
    <contain>1272264863.8487182</contain>
    <laid expression=""nine"">2026401752.6170769
      <!-- article deeply sand rhyme explore helpful -->
    </laid>
  </caught>
</root>");

	class ParserResult
	{
		public int NumOfStartElementEvents;
		public int NumOfAttributeEvents;
		public int NumOfEndElementEvents;
		public int NumOfCommentEvents;
		public int NumOfCdataEvents;
		public int NumOfAllTextEvents;
		public int NumOfUniqueTextEvents;
	}

	enum NodeType
	{
		None,
		Element,
		Text,
		Comment,
		Cdata
	}

	static ParserResult ParseXmlFromBuffer(byte[] buf)
	{
		var res = new ParserResult();

		using var ms = new MemoryStream();
		ms.Write(buf);
		ms.Position = 0;

		using var p = new XmppTokenizer();

		int depth = 0;

		var tabs = () => new string('\t', Math.Max(0, depth));
		var realText = (string s) => s.Trim(' ', '\r', '\n', '\t');

		var lastNode = NodeType.None;

		p.OnElementStart += (name, attrs) =>
		{
			Console.WriteLine($"{tabs()} Element(start): {name}");
			Console.WriteLine(tabs());
			Console.WriteLine($"{tabs()}  # Attributes({attrs.Count}):");

			res.NumOfAttributeEvents += attrs.Count;

			foreach (var (attName, attVal) in attrs)
				Console.WriteLine($"{tabs()}   Attribute: name={attName}; value={attVal}");

			Console.WriteLine(tabs());
			depth++;

			res.NumOfStartElementEvents++;
			lastNode = NodeType.Element;
		};

		p.OnElementEnd += (name) =>
		{
			Console.WriteLine(tabs());

			if (depth > 0)
				depth--;

			Console.WriteLine($"{tabs()} Element(end): {name}");

			res.NumOfEndElementEvents++;
		};

		p.OnText += (value) =>
		{
			var t = realText(value);

			Console.WriteLine($"{tabs()} Text: {t}");

			if (!string.IsNullOrWhiteSpace(t))
			{
				if (lastNode != NodeType.Text)
				{
					lastNode = NodeType.Text;
					res.NumOfUniqueTextEvents++;
				}
			}

			res.NumOfAllTextEvents++;
		};

		p.OnCdata += (value) =>
		{
			var t = realText(value);

			if (!string.IsNullOrWhiteSpace(t))
				Console.WriteLine($"{tabs()} Cdata: {t}");

			res.NumOfCdataEvents++;
		};

		p.OnComment += value =>
		{
			var t = realText(value);

			if (!string.IsNullOrWhiteSpace(t))
				Console.WriteLine($"{tabs()} Comment: {value.Trim('\r', '\n', '\t', ' ')}");

			res.NumOfCommentEvents++;
		};

		int cnt;

		var b = new byte[4];

		while ((cnt = ms.Read(b, 0, b.Length)) > 0)
			p.Write(b, cnt);

		return res;
	}

	[TestMethod]
	public void TestStartElement()
	{
		var r = ParseXmlFromBuffer(StartTagData);
		Assert.AreEqual(1, r.NumOfStartElementEvents);
		Assert.AreEqual(2, r.NumOfAttributeEvents);
	}

	[TestMethod]
	public void TestEndElement()
	{
		var r = ParseXmlFromBuffer(EndTagData);
		Assert.AreEqual(1, r.NumOfEndElementEvents);
	}

	[TestMethod]
	public void TestStartEndTagXML()
	{
		var r = ParseXmlFromBuffer(ElementData);
		Assert.AreEqual(1, r.NumOfStartElementEvents);
		Assert.AreEqual(2, r.NumOfAttributeEvents);
		Assert.AreEqual(1, r.NumOfEndElementEvents);
	}

	[TestMethod]
	public void TestRealXML()
	{
		var r = ParseXmlFromBuffer(FullTestData);

		Assert.AreEqual(7, r.NumOfStartElementEvents);
		Assert.AreEqual(7, r.NumOfEndElementEvents);
		Assert.AreEqual(1, r.NumOfCommentEvents);
		Assert.AreEqual(2, r.NumOfCdataEvents);

		// Chunked text events is not accurated, if we increase/decrease buffer size while parsing, this count will change too.
		// This is not same behaviour in Comment and Cdata nodes, because its expects OPEN and CLOSE tokens.
		Assert.AreEqual(5, r.NumOfUniqueTextEvents);
		Assert.AreEqual(58, r.NumOfAllTextEvents);
	}

	[TestMethod]
	public void ParseEntityrefTokens()
	{
		var buf = "<foo xmlns='bar'>&amp;amp;&#x20;&#x22;</foo>"u8.ToArray();
		ParseXmlFromBuffer(buf);
	}

	[TestMethod]
	public async Task ParseAsXElement()
	{
		using var tokenizer = new XmppTokenizer();

		XElement? root = default;

		tokenizer.OnElementStart += (name, attrs) =>
		{
			var (localName, prefix) = name;

			var uri = tokenizer.LookupNamespace(prefix) ?? XNamespace.None;

			var element = new XElement(uri + localName);

			if (!string.IsNullOrEmpty(prefix))
				element.Add(new XAttribute(XNamespace.Xmlns + prefix, uri.NamespaceName));

			foreach (var (key, value) in attrs)
			{
				if (!key.HasPrefix)
					element.SetAttributeValue(key.LocalName, value);
				else
				{
					XNamespace ns = key.Prefix switch
					{
						"xml" => XNamespace.Xml,
						"xmlns" => XNamespace.Xmlns,
						_ => tokenizer.LookupNamespace(key.Prefix) ?? string.Empty
					};

					element.SetAttributeValue(ns + key.LocalName, value);
				}
			}

			root?.Add(element);
			root = element;
		};

		tokenizer.OnElementEnd += (name) =>
		{
			var parent = root?.Parent;

			if (parent != null)
				root = parent;
		};

		tokenizer.OnText += text =>
		{
			var value = text.TrimWhitespaces();

			if (root != null)
			{
				if (root.LastNode is XText node)
					node.Value += value;
				else
					root.Add(new XText(value));
			}
		};

		tokenizer.OnCdata += value =>
		{
			root?.Add(new XCData(value));
		};

		tokenizer.OnComment += value =>
		{
			root?.Add(new XComment(value));
		};

		var task = Task.Run(async () =>
		{
			using var ms = new MemoryStream();
			ms.Write(FullTestData);
			ms.Position = 0;

			var buf = new byte[5];
			int cnt;

			while ((cnt = await ms.ReadAsync(buf)) > 0)
			{
				tokenizer.Write(buf, cnt);
				await Task.Delay(1);
			}
		});

		var watch = Stopwatch.StartNew();

		while (!task.IsCompleted)
			await Task.Delay(1);

		Console.WriteLine();

		Assert.IsNotNull(root);

		Console.WriteLine(root.ToString(SaveOptions.OmitDuplicateNamespaces));
	}
}
