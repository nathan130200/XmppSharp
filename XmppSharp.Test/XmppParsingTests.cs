using System.Net;
using System.Net.Sockets;
using System.Text;
using XmppSharp.Dom;
using XmppSharp.Parser;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp;

[TestClass]
public class XmppParsingTests
{
    #region XmppStreamReader Parser Test

    [TestMethod]
    public void ParseFromSocket_XmppStreamReader()
    {
        using var server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Any, 5222));
        server.Listen(1);

        Console.WriteLine("begin accept client");

        using var client = server.Accept();
        Assert.IsNotNull(client);

        using var stream = new NetworkStream(client);
        using var reader = new XmppStreamReader(stream);

        Console.WriteLine("setup xmpp parser");

        StreamStream? streamElement = null;
        Element? authElement = null;

        reader.OnStreamStart += (e) =>
        {
            Console.WriteLine("start tag received: " + e.StartTag());

            streamElement = e;

            e.SwitchDirection();

            Send(e.StartTag());

            var features = new StreamFeatures
            {
                Mechanisms = new Mechanisms
                {
                    SupportedMechanisms = new[]
                    {
                        new Mechanism("PLAIN")
                    }
                }
            };

            Send(features.ToString());
        };

        reader.OnStreamElement += e =>
        {
            if (e is Auth auth)
            {
                Console.WriteLine("auth received: " + e.ToString());
                authElement = auth;
            }
            else
            {
                Console.WriteLine("unknown element received: " + e.ToString());
            }
        };

        var timeout = Task.Delay(TimeSpan.FromMinutes(1));
        var counter = 0;

        while (true)
        {
            Thread.Sleep(1);

            if (authElement != null && streamElement != null)
            {
                Console.WriteLine("parsing test completed & passed");
                break;
            }

            var result = reader.Advance();

            if (result)
                Console.WriteLine("advance parser... [{0}]", counter++);
            else
            {
                Assert.Fail("Not enough XML data to parse?");
                return;
            }
        }

        Assert.IsFalse(timeout.IsCompleted); // Should never happen, connect XMPP client to test server ASAP test start!
        Assert.IsNotNull(streamElement);
        Assert.IsNotNull(authElement);

        void Send(string data)
        {
            lock (stream)
            {
                stream.Write(Encoding.UTF8.GetBytes(data));
            }
        }
    }

    #endregion

    #region XmppStreamReader Parser Test

    [TestMethod]
    public void ParseFromSocket_ExpatParser()
    {
        using var server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Any, 5222));
        server.Listen(1);

        Console.WriteLine("begin accept client");

        using var client = server.Accept();
        Assert.IsNotNull(client);

        using var stream = new NetworkStream(client);
        using var reader = new ExpatXmppParser(Encoding.UTF8);

        Console.WriteLine("setup xmpp parser");

        StreamStream? streamElement = null;
        Element? authElement = null;

        reader.OnStreamStart += (e) =>
        {
            Console.WriteLine("start tag received: " + e.StartTag());

            streamElement = e;

            e.SwitchDirection();

            Send(e.StartTag());

            var features = new StreamFeatures
            {
                Mechanisms = new Mechanisms
                {
                    SupportedMechanisms = new[]
                    {
                        new Mechanism("PLAIN")
                    }
                }
            };

            Send(features.ToString());
        };

        reader.OnStreamElement += e =>
        {
            if (e is Auth auth)
            {
                Console.WriteLine("auth received: " + e.ToString());
                authElement = auth;
            }
            else
            {
                Console.WriteLine("unknown element received: " + e.ToString());
            }
        };

        var timeout = Task.Delay(TimeSpan.FromMinutes(1));
        var counter = 0;

        byte[] buf = new byte[256];
        int len;

        while (true)
        {
            Thread.Sleep(1);

            if (authElement != null && streamElement != null)
            {
                Console.WriteLine("parsing test completed & passed");
                break;
            }

            len = stream.Read(buf);

            Console.WriteLine("recv (num bytes): " + len);

            if (len == 0)
                break;

            reader.Write(buf, len);
            Console.WriteLine("advance parser... [{0}]", counter++);
        }

        reader.Write(buf, 0, true);

        Assert.IsFalse(timeout.IsCompleted); // Should never happen, connect XMPP client to test server ASAP test start!
        Assert.IsNotNull(streamElement);
        Assert.IsNotNull(authElement);

        void Send(string data)
        {
            lock (stream)
            {
                stream.Write(Encoding.UTF8.GetBytes(data));
            }
        }
    }

    #endregion
}
