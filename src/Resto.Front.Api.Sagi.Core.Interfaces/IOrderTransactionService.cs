using System;
using Resto.Front.Api.Sagi.Domain.Branches;
using Resto.Front.Api.Sagi.Domain.Orders;

namespace Resto.Front.Api.Sagi.Core.Interfaces
{
	/// <summary>
	///     Сервис для работы с заказом.
	/// </summary>
	public interface IOrderTransactionService : IDisposable
	{
		/// <summary>
		///     Поиск клиента по 6-значному коду.
		/// </summary>
		BranchCustomer FindBranchCustomerByCode(string code);

		/// <summary>
		///     Поиск клиента по номеру телефона.
		/// </summary>
		BranchCustomer FindBranchCustomerByPhoneNumber(string phoneNumber);

		/// <summary>
		///     Получить транзакцию заказа.
		/// </summary>
		/// <param name="orderId">Идентификатор заказа iiko.</param>
		OrderTransaction GetOrderTransactionById(string orderId);

		/// <summary>
		///     Создать транзакцию.
		/// </summary>
		void CreateOrderTransaction(OrderTransaction orderTransaction);

		/// <summary>
		///     Завершаем транзакцию.
		/// </summary>
		void CommitTransaction(string orderId, string paymentMethod = "CASH");

		/// <summary>
		///     Откат транзакции.
		/// </summary>
		void RollbackTransaction(string orderId);
	}
}