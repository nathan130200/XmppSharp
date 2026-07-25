using System.Net;

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

};

component.Connect(IPEndPoint.Parse("127.0.0.1:5275"), "example.warface", "youshallnotpass");

Console.ReadLine();