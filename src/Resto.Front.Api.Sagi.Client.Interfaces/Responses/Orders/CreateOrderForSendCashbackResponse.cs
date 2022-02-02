using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Orders
{
	public class CreateOrderForSendCashbackResponse
	{
		[JsonProperty("id")] public long Id { get; set; }
	}
}