using XmppSharp.Entities;

var settings = new XmppServerConfiguration
{
	Tls =
	{
		Policy = TlsPolicy.Required,
		UseSelfSignedCert = true,
	}
};