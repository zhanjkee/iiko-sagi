using Resto.Front.Api.Sagi.Client.Interfaces;

namespace Resto.Front.Api.Sagi.Client
{
	public class SagiRestClientFactory : ISagiRestClientFactory
	{
		/// <inheritdoc />
		public ISagiRestClient Create(string webAddress)
		{
			return new SagiRestClient(webAddress);
		}
	}
}