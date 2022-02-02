namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Branches
{
	public class GetBalanceForBranchRequest
	{
		public long BranchId { get; set; }
		public long UserId { get; set; }
		public long GroupId { get; set; }
	}
}