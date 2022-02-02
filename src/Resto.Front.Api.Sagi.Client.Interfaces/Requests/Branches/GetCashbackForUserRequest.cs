namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Branches
{
	public class GetCashbackForUserRequest
	{
		public long BranchId { get; set; }
		public long UserId { get; set; }
	}
}