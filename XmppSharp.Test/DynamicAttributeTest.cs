using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmppSharp.Dom;

namespace XmppSharp.Test;

[TestClass]
public class DynamicAttributeTest
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
}
