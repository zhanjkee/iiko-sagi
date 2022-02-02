using Newtonsoft.Json;

namespace Resto.Front.Api.Sagi.Client.Interfaces.Responses.Branches
{
	public class Rule
	{
		[JsonProperty("cashback_percentage")] public decimal CashbackPercentage { get; set; }
	}
}