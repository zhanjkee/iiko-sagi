using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Auth
{
	public class Organization
	{
		[JsonProperty("id")] public int Id { get; set; }

		[JsonProperty("role")] public string Role { get; set; }

		[JsonProperty("branches")] public long[] Branches { get; set; }
	}
}