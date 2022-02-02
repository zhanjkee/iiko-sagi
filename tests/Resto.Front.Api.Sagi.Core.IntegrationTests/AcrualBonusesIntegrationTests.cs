using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resto.Front.Api.Sagi.Domain.Orders;

namespace Resto.Front.Api.Sagi.Core.IntegrationTests
{
	[TestClass]
	public class OrderTransactionServiceUnitTests : BaseIntegrationTests
	{
		[TestMethod]
		public void CreateOrderTransaction_AccrualBonuses()
		{
			IikoOrderId = RandomString(10);

			var orderTransaction = new OrderTransaction
			{
				Amount = Amount,
				PaymentMethod = PaymentMethod,
				IikoOrderId = IikoOrderId,
				Detail = new OrderTransactionDetail
				{
					BranchCustomer = OrderTransactionService.FindBranchCustomerByPhoneNumber(CustomerPhoneNumber)
				}
			};

			if (orderTransaction.CanGiveAward())
			{
				orderTransaction.GiveAward = true;
			}
			else
			{
				orderTransaction.AddStamp = true;
				orderTransaction.StampCount = 2;
			}

			var branchCustomer = orderTransaction.Detail.BranchCustomer;
			OrderTransactionService.CreateOrderTransaction(orderTransaction);

			Assert.IsNotNull(branchCustomer);
			Assert.IsNotNull(branchCustomer.Customer);
			Assert.IsNotNull(branchCustomer.Award);
			Assert.IsNotNull(branchCustomer.Cashback);
		}

		[TestMethod]
		public void CommitOrderTransaction_AccrualBonuses()
		{
			OrderTransactionService.CommitTransaction(IikoOrderId);

			Assert.IsTrue(true);
		}

		[TestMethod]
		public void RevertOrderTransaction_AccrualBonuses()
		{
			OrderTransactionService.RollbackTransaction(IikoOrderId);

			Assert.IsTrue(true);
		}
	}
}