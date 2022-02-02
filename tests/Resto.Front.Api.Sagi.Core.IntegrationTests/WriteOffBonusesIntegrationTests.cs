using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resto.Front.Api.Sagi.Domain.Orders;

namespace Resto.Front.Api.Sagi.Core.IntegrationTests
{
	[TestClass]
	public class WriteOffBonusesIntegrationTests : BaseIntegrationTests
	{
		[TestMethod]
		public void CreateOrderTransaction_WriteOffBonuses()
		{
			IikoOrderId = RandomString(10);

			var orderTransaction = new OrderTransaction
			{
				Amount = Amount,
				PaymentMethod = PaymentMethod,
				IikoOrderId = IikoOrderId,
				Detail = new OrderTransactionDetail
				{
					BranchCustomer = OrderTransactionService.FindBranchCustomerByCode(CustomerCode)
				},
				WriteOffSum = 100
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
		public void CommitOrderTransaction_WriteOffBonuses()
		{
			OrderTransactionService.CommitTransaction(IikoOrderId);

			Assert.IsTrue(true);
		}

		[TestMethod]
		public void RevertOrderTransaction_WriteOffBonuses()
		{
			OrderTransactionService.RollbackTransaction(IikoOrderId);

			Assert.IsTrue(true);
		}
	}
}
