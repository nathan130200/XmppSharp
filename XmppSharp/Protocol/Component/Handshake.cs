using System.Security.Cryptography;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Component;

[Tag("handshake", Namespaces.Component)]
public class Handshake : XmppElement
{
	public Handshake() : base("handshake", Namespaces.Component)
	{

	}

	public Handshake(string streamId, string password) : this()
	{
		InnerText = GetAuthenticationHash(streamId, password);
	}

	public static string GetAuthenticationHash(string streamId, string password)
	{
		var hash = SHA1.HashData(string.Concat(streamId, password).GetBytes());
		return hash.GetHexLower();
	}
}