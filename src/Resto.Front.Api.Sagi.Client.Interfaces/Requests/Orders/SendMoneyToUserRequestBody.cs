using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Orders
{
	public class SendMoneyToUserRequestBody
	{
		[JsonProperty("use_balance")] public bool UseBalance { get; set; }

		[JsonProperty("amount")] public int Amount { get; set; }

		[JsonProperty("payment_method")] public string PaymentMethod { get; set; }
	}
}