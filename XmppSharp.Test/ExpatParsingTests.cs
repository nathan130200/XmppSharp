#nullable enable annotations

using Expat;

namespace XmppSharp;

// Pra ser sincero esse codigo nem deveria estar aqui mais, afinal
// a lib Expat.Net já tem testes também (que estão passando).

[TestClass]
public class ExpatParsingTests
{
    [TestMethod]
    public void ParseFromBuffer()
    {
        string? tagName = null;
        string? firstAttr = null;
        string? textContent = default,
            commentContent = default,
            cdataContent = null;

        using var parser = new ExpatParser();
        {
            parser.OnStartElement += (name, attrs) =>
            {
                Console.WriteLine("tag name: " + name);
                Console.WriteLine("attr count: " + attrs.Count);

                foreach (var (key, value) in attrs)
                    Console.WriteLine(" -- attr: {0} -> {1}", key, value);

                tagName = name;
                firstAttr = attrs.GetValueOrDefault("xmlns");
            };

            parser.OnText += e =>
            {
                Console.WriteLine("node <text>: " + e);
                textContent = e;
            };

            parser.OnCdata += e =>
            {
                Console.WriteLine("node <cdata>: " + e);
                cdataContent = e;
            };

            parser.OnComment += e =>
            {
                Console.WriteLine("node <comment>: " + e);
                commentContent = e;
            };

            var xml = "<foo xmlns='bar'>my text<![CDATA[my cdata]]><!--my comment--></foo>".GetBytes();

            parser.Parse(xml, xml.Length);
        }

        Assert.AreEqual("foo", tagName);
        Assert.AreEqual("bar", firstAttr);
        Assert.AreEqual("my text", textContent);
        Assert.AreEqual("my cdata", cdataContent);
        Assert.AreEqual("my comment", commentContent);
    }
}
