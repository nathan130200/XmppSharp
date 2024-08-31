using XmppSharp.Dom;
using XmppSharp.Protocol;

namespace XmppSharp.Test;

[TestClass]
public class DynamicBindingTests
{
	[TestMethod]
	public void TestWithPrimitives()
	{
		var xml = Xml.Parse("<foo my_int='2' my_float='1.25' />");

		int myInt = xml.attrs.my_int;
		Assert.AreEqual(2, myInt);

		float myFloat = xml.attrs.my_float;
		Assert.AreEqual(1.25f, myFloat);
	}

	[TestMethod]
	public void TestNumChild()
	{
		var element = Xml.Parse("<foo/>");

		var bar = () => Xml.Element("bar");
		element.AddChild(bar());
		element.AddChild(bar());
		element.AddChild(bar());
		element.AddChild(bar());

		var count = element.Children("bar").Count();

		Assert.AreEqual(4, count);

		Console.WriteLine("OUT:\n" + element.ToString(XmlFormatting.Indented) + "\n");
	}

	[TestMethod]
	public void CreateFromCtor()
	{
		/*
		var el = new Iq(IqType.Result)
		{
			children =
			{
				new Element("query", Namespaces.IqRpc)
				{
					attrs =
					{
						command_id = 12345,
					},
					children =
					{
						"some cool text"
					}
				}
			}
		};
		*/

		var el = new Iq(IqType.Result);
		{
			var rpc = el.C("query", Namespaces.IqRpc);
			rpc.attrs.command_id = 12345;
			rpc.Value = "some cool text";
		}

		var xml = el.ToString(true);

		Console.WriteLine("OUT:\n{0}\n", xml);

		Assert.AreEqual("query", el.FirstChild!.TagName);
		Assert.AreEqual(12345, (int)el.FirstChild!.attrs.command_id);
		Assert.IsTrue(el.FirstChild.LastNode is Text);
		Assert.AreEqual("some cool text", el.FirstChild.LastNode.Value);
	}
}
