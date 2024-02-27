using XmppSharp.Entities;
using XmppSharp.Net;

var config = new XmppServerConfiguration
{
	Tls =
	{
		Policy = TlsPolicy.Required,
		UseSelfSignedCert = true,
	}
};

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
	e.Cancel = true;

	if (!cts.IsCancellationRequested)
		cts.Cancel();
};

try
{
	var server = new XmppServer(config);
	await server.StartAsync(cts.Token);

	server.OnClientConnected += session =>
	{
		session.OnReadXml += e =>
		{
			Console.WriteLine("recv <<\n{0}\n", e);
		};

		session.OnWriteXml += (xml, ex) =>
		{
			Console.WriteLine("send >>\n{0}{1}\n", (ex != null ? ex.ToString() + "\n\n" : ""), xml);
		};

		return Task.CompletedTask;
	};

	while (!cts.IsCancellationRequested)
		await Task.Delay(1);

	await server.StopAsync();
}
catch (Exception e)
{
	Console.WriteLine(e);
	await Task.Delay(-1);
}

await Task.Delay(2500);
