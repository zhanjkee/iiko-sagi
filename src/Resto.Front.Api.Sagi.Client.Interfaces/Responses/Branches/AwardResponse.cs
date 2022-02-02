using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches
{
	public class AwardResponse
	{
		[JsonProperty("id")] public string Id { get; set; }

		[JsonProperty("received_stamp_count")] public int ReceivedStampCount { get; set; }

		[JsonProperty("stamp_count")] public int StampCount { get; set; }
	}
}