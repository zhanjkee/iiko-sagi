namespace Resto.Front.Api.Sagi.Client.Interfaces.Requests.Branches
{
	public class GiveAwardRequest
	{
		public long BranchId { get; set; }
		public string AwardId { get; set; }
		public GiveAwardRequestBody Body { get; set; }
	}
}