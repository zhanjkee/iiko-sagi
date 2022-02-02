using System;
using Resto.Front.Api.Sagi.Entities.Base;

namespace Resto.Front.Api.Sagi.Entities
{
	/// <summary>
	///     Транзакция заказа.
	/// </summary>
	public class OrderTransactionEntity : BaseEntity
	{
		/// <summary>
		///     Дата создания транзакции.
		/// </summary>
		public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

		/// <summary>
		///     Идентификатор заказа Iiko.
		/// </summary>
		public string IikoOrderId { get; set; }

		/// <summary>
		///     Идентификатор заказа Sagi.
		/// </summary>
		public long SagiOrderId { get; set; }

		/// <summary>
		///     Сумма заказа iiko.
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		///     Количество списываемых бонусов клиента.
		/// </summary>
		public int? WriteOffSum { get; set; }

		/// <summary>
		///     Выдать награду.
		/// </summary>
		public bool AddStamp { get; set; }

		/// <summary>
		///     Количество начисляемых штампиков.
		/// </summary>
		public int StampCount { get; set; }

		/// <summary>
		///     Выдать подарок.
		/// </summary>
		public bool GiveAward { get; set; }

		/// <summary>
		///     Метод платежа.
		/// </summary>
		public string PaymentMethod { get; set; }

		/// <summary>
		///     Детали заказа.
		/// </summary>
		/// <remarks>В формате Json.</remarks>
		public string TransactionDetail { get; set; }
	}
}