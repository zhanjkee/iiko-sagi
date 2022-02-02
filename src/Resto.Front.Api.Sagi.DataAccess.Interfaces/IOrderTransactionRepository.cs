using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.Interfaces
{
	/// <summary>
	///     Репозитории транзакции заказов.
	/// </summary>
	public interface IOrderTransactionRepository
	{
		/// <summary>
		///     Получить транзакцию по идентификатору заказа iiko.
		/// </summary>
		OrderTransactionEntity GetOrderTransactionByIikoOrderId(string iikoOrderId);

		/// <summary>
		///     Создать транзакцию заказа.
		/// </summary>
		void CreateOrderTransaction(OrderTransactionEntity order);

		/// <summary>
		///     Обновить транзакцию заказа.
		/// </summary>
		void UpdateOrderTransaction(OrderTransactionEntity order);

		/// <summary>
		///     Удалить транзакцию.
		/// </summary>
		void DeleteOrderTransaction(int id);
	}
}