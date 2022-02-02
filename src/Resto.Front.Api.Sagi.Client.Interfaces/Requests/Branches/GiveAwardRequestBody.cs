using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Branches
{
	public class GiveAwardRequestBody
	{
		[JsonProperty("user_id")] public long UserId { get; set; }

		[JsonProperty("stamp_count")] public int StampCount { get; set; }
	}
}