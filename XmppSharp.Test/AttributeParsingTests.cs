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
        Assert.IsFalse(cond);
    }

    [TestMethod]
    public void ShouldBeNotNull()
    {
        var el = new XmppElement("sample")
            .SetAttribute("count", 1);

        var item = el.GetAttributeInt32("count");
        Assert.AreEqual(1, item);
    }

    [TestMethod]
    public void ShouldParseTheValues()
    {
        var el = new XmppElement("sample")
            .SetAttribute("my_float", 1.25f)
            .SetAttribute("my_double", double.Epsilon)
            .SetAttribute("my_bool_as_int", 1)
            .SetAttribute("my_bool_as_string", "false");

        Console.WriteLine(el.ToString());

        var myFloat = el.GetAttributeFloat("my_float", 0f);
        Assert.AreEqual(1.25f, myFloat);

        var myDouble = el.GetAttributeDouble("my_double", 0d);
        Assert.AreEqual(double.Epsilon, myDouble);

        var myBool = el.GetAttributeBool("my_bool_as_int", false);
        Assert.AreEqual(true, myBool);

        myBool = el.GetAttributeBool("my_bool_as_string", true);
        Assert.AreEqual(false, myBool);
    }
}
