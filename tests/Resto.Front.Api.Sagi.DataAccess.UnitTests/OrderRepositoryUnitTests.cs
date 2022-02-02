using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resto.Front.Api.Sagi.DataAccess.UoW;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.UnitTests
{
	[TestClass]
	public class OrderRepositoryUnitTests
	{
		private const string IikoOrderId = "EFC79563-AA02-4800-91B9-EBBF8BC7CBA3";

		[TestMethod]
		[Priority(1)]
		public void AddOrderTransactionUnitTests()
		{
			var order = new OrderTransactionEntity
			{
				IikoOrderId = IikoOrderId,
				SagiOrderId = 2,
				WriteOffSum = 1000,
				AddStamp = true,
				Amount = 2000,
				GiveAward = true,
				PaymentMethod = "CASH",
				TransactionDetail = "Some detail"
			};

			using (var unitOfWork = new UnitOfWork())
			{
				var orderTransactionRepository = unitOfWork.OrderTransactionRepository;
				orderTransactionRepository.CreateOrderTransaction(order);
				unitOfWork.SaveChanges();
			}

			Assert.IsTrue(true);
		}

		[TestMethod]
		[Priority(2)]
		public void GetOrderTransactionUnitTests()
		{
			OrderTransactionEntity orderTransactionEntity;
			using (var unitOfWork = new UnitOfWork())
			{
				var orderTransactionRepository = unitOfWork.OrderTransactionRepository;
				orderTransactionEntity = orderTransactionRepository.GetOrderTransactionByIikoOrderId(IikoOrderId);
			}

			Assert.IsNotNull(orderTransactionEntity);
			Assert.AreEqual("CASH", orderTransactionEntity.PaymentMethod);
		}

		[TestMethod]
		[Priority(3)]
		public void DeleteOrderTransactionUnitTests()
		{
			using (var unitOfWork = new UnitOfWork())
			{
				var orderTransactionRepository = unitOfWork.OrderTransactionRepository;
				var orderTransactionEntity = orderTransactionRepository.GetOrderTransactionByIikoOrderId(IikoOrderId);
				orderTransactionRepository.DeleteOrderTransaction(orderTransactionEntity.Id);
				unitOfWork.SaveChanges();
			}

			Assert.IsTrue(true);
		}

		[TestMethod]
		[Priority(4)]
		public void GetOrderTransactionMustNullUnitTests()
		{
			OrderTransactionEntity orderTransactionEntity;
			using (var unitOfWork = new UnitOfWork())
			{
				var orderTransactionRepository = unitOfWork.OrderTransactionRepository;
				orderTransactionEntity = orderTransactionRepository.GetOrderTransactionByIikoOrderId(IikoOrderId);
			}

			Assert.IsNull(orderTransactionEntity);
		}
	}
}
