namespace XmppSharp.Entities;

public class TlsConfiguration
{
	/// <summary>
	/// Defines what the encryption policy is on the server.
	/// </summary>
	public TlsPolicy Policy { get; set; } = TlsPolicy.Required;

	/// <summary>
	/// Determines whether we should use a self-signed certificate.
	/// </summary>
	public bool UseSelfSignedCert { get; set; } = true;
	public string? CertificatePath { get; set; }
	public string? CertificatePassword { get; set; }
}
