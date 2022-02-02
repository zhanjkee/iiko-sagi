namespace Resto.Front.Api.Sagi.Client.Interfaces
{
	public interface ISagiRestClientFactory
	{
		ISagiRestClient Create(string webAddress);
	}
}