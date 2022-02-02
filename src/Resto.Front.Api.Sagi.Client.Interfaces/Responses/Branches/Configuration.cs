using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches
{
	public class Configuration
	{
		[JsonProperty("award")] public Award Award { get; set; }
	}
}