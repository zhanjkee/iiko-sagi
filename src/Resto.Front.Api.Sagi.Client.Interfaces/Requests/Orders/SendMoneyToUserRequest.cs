namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Orders
{
	public class SendMoneyToUserRequest
	{
		public long OrderId { get; set; }
		public SendMoneyToUserRequestBody Body { get; set; }
	}
}