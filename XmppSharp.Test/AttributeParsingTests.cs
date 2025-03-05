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

        var myFloat = el.GetAttributeFloat("my_float", 0f);
        Assert.AreEqual(1.25f, myFloat);

        var myDouble = el.GetAttributeDouble("my_double", 0d);
        Assert.AreEqual(double.Epsilon, myDouble);

        var myBool = el.GetAttributeBool("my_bool_as_int") ?? false;
        Assert.AreEqual(true, myBool);

        myBool = el.GetAttributeBool("my_bool_as_string") ?? true;
        Assert.AreEqual(false, myBool);
    }

    /*[TestMethod]
    public void ShouldParseOtherValues()
    {
        var clockRate = TimeSpan.FromSeconds(3.5d);
        var dateTime = new DateTime(2024, 6, 12, 0, 12, 24);
        var dateTimeOffset = new DateTimeOffset(2024, 6, 12, 0, 12, 24, TimeSpan.FromHours(-3));

        var elem = new XmppElement("sample")
            .SetAttribute("i16", 16)
            .SetAttribute("i32", 32)
            .SetAttribute("i64", 64)

            .SetAttribute("u16", 16U)
            .SetAttribute("u32", 32U)
            .SetAttribute("u64", 64U)

            .SetAttribute("ts", clockRate)
            .SetAttribute("date_time", dateTime)
            .SetAttribute("date_time_offset", dateTimeOffset)
            ;

        TestValue("i16", (short)16);
        TestValue("i32", 32);
        TestValue("i64", 64L);
        TestValue("u16", (ushort)16);
        TestValue("u32", 32U);
        TestValue("u64", 64UL);
        TestValue("ts", clockRate);
        TestValue("date_time", dateTime);
        TestValue("date_time_offset", dateTimeOffset);

        Console.WriteLine(elem.ToString());

        void TestValue<T>(string attrName, T expected) where T : struct
        {
            Console.WriteLine("testing: {0} (type: {1})", attrName, typeof(T));
            var temp = elem.GetAttribute<T>(attrName);
            Console.WriteLine("getAttr(): " + temp);
            Assert.IsTrue(temp.HasValue);
            Assert.AreEqual(expected, temp.Value);
            Console.WriteLine("test value <{0}>: {1} == {2}", expected, typeof(T), temp);
        }
    }*/
}
