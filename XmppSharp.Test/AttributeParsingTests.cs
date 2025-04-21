using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmppSharp.Dom;
using XmppSharp.Extensions;

namespace XmppSharp;

[TestClass]
public class AttributeParsingTests
{
    [TestMethod]
    public void ShouldReturnNull()
    {
        var el = new XmppElement("test");
        el.SetAttribute("foo", "bar");

        var cond = el.GetAttributeBool("checked");
        Assert.IsNull(cond);
    }

    [TestMethod]
    public void ShouldBeNotNull()
    {
        var el = new XmppElement("sample");
        el.SetAttribute("count", 1);

        var item = el.GetAttribute<int>("count");
        Assert.AreEqual(1, item);
    }

    [TestMethod]
    public void ShouldParseTheValues()
    {
        var el = new XmppElement("sample");
        el.SetAttribute("my_float", 1.25f);
        el.SetAttribute("my_double", double.Epsilon);
        el.SetAttribute("my_bool_as_int", 1);
        el.SetAttribute("my_bool_as_string", "false");
        ;

        Console.WriteLine(el.ToString());

        var myFloat = el.GetAttribute("my_float", 0f);
        Assert.AreEqual(1.25f, myFloat);

        var myDouble = el.GetAttribute("my_double", 0d);
        Assert.AreEqual(double.Epsilon, myDouble);

        var myBool = el.GetAttributeBool("my_bool_as_int");
        Assert.AreEqual(true, myBool);

        myBool = el.GetAttributeBool("my_bool_as_string");
        Assert.AreEqual(false, myBool);
    }
}
