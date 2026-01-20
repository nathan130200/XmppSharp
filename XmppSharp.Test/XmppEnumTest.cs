using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmppSharp.Protocol;

namespace XmppSharp;

[TestClass]
public class XmppEnumTest
{
	[TestMethod]
	public void Parse_WillThrow()
	{
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => XmppEnum.Parse<IqType>("invalid"));
	}

	[TestMethod]
	public void Parse_NoThrow()
	{
		Parse_NoThrow_Case("get", IqType.Get);
		Parse_NoThrow_Case("error", IqType.Error);
		Parse_NoThrow_Case("xa", PresenceShow.ExtendedAway);
		Parse_NoThrow_Case("dnd", PresenceShow.DoNotDisturb);

		void Parse_NoThrow_Case<T>(string str, T value) where T : struct, Enum
		{
			var current = XmppEnum.Parse<T>(str);
			Assert.AreEqual(value, current);
		}
	}
}
