namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Orders
{
	public class CreateOrderForSendCashbackRequest
	{
		public long BranchId { get; set; }
		public CreateOrderForSendCashbackRequestBody Body { get; set; }
	}
}