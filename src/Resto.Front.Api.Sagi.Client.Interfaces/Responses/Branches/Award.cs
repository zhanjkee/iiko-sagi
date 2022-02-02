using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches
{
	public class Award
	{
		[JsonProperty("enabled")] public bool Enabled { get; set; }

		[JsonProperty("stamp_count")] public int StampCount { get; set; }
	}
}