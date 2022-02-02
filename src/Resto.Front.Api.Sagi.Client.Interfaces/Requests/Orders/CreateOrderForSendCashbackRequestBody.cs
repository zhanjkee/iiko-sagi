using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Orders
{
	public class CreateOrderForSendCashbackRequestBody
	{
		[JsonProperty("user_id")] public long UserId { get; set; }

		[JsonProperty("amount")] public int Amount { get; set; }

		[JsonProperty("comment")] public string Comment { get; set; }

		[JsonProperty("user_phone")] public string UserPhone { get; set; }

		[JsonProperty("add_stamp")] public bool AddStamp { get; set; }

		[JsonProperty("stamp_count")] public int StampCount { get; set; }

		[JsonProperty("give_award")] public bool GiveAward { get; set; }
	}
}