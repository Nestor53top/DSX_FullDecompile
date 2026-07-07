using System.Net;

namespace NuGet;

public class CredentialResult
{
	public CredentialState State { get; set; }

	public ICredentials Credentials { get; set; }

	public static CredentialResult Create(CredentialState state, ICredentials credentials)
	{
		return new CredentialResult
		{
			State = state,
			Credentials = credentials
		};
	}
}
