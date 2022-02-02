using System;
using Resto.Front.Api.Sagi.Domain.Branches;

namespace Resto.Front.Api.Sagi.Domain.Orders
{
	public class OrderTransactionDetail
	{
		public BranchCustomer BranchCustomer { get; set; }
		public bool WasRefund { get; set; } = false;
		public DateTime? RefundDateTimeUtc { get; set; }
	}
}