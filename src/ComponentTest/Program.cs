using System.Net;
using XmppSharp;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;

using var component = new Component();

Environment.SetEnvironmentVariable("EXPAT_LIBRARY_PATH", @"C:\vcpkg\installed\x64-windows\bin\libexpat.dll");

component.OnReadXml += xml => Console.WriteLine("recv <<\n{0}\n", xml);

component.OnWriteXml += xml => Console.WriteLine("send >>\n{0}\n", xml);

component.OnError += ex =>
{
	Console.WriteLine("Uncaught exception:" + ex);
};

component.OnElement += e =>
{
	if (e is Stanza stz)
	{
		stz.SwitchDirection();

		stz.SetAttribute("type", XmppEnum<IqType>.GetName(IqType.Error));

		stz.Error = new Error()
		{
			Type = ErrorType.Cancel,
			Condition = ErrorCondition.FeatureNotImplemented
		};

		component.Send(stz);
	}
};

component.Connect(IPEndPoint.Parse("127.0.0.1:5275"), "example.warface", "youshallnotpass");

Console.ReadLine();