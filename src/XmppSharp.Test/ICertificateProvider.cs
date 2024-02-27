using System.Security.Cryptography.X509Certificates;

namespace XmppSharp.Test;

public interface ICertificateProvider : IDisposable
{
	Task<X509Certificate2> ProvideAsync(CancellationToken token = default);
}
