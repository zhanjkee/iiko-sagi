namespace Resto.Front.Api.Sagi.Domain.Orders
{
	public class OrderTransaction
	{
		/// <summary>
		///     Идентификатор заказа в БД.
		/// </summary>
		public int Id { get; set; }

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
		public OrderTransactionDetail Detail { get; set; }

		/// <summary>
		///     True, если можно выдать подарок клиенту.
		/// </summary>
		public bool CanGiveAward()
		{
			return Detail.BranchCustomer.Award.StampCount != 0 &&
			       Detail.BranchCustomer.Award.StampCount <= Detail.BranchCustomer.Customer.ReceivedStampCount;
		}

		/// <summary>
		///     Получить количество начисляемых бонусов.
		/// </summary>
		public decimal GetAccrualAmount()
		{
			return Amount / 100 * Detail?.BranchCustomer?.Cashback?.CashbackPercentage ?? 0;
		}
	}
}