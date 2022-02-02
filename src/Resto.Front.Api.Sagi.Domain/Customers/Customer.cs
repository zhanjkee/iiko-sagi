namespace Resto.Front.Api.Sagi.Domain.Customers
{
	public class Customer
	{
		/// <summary>
		///     Идентификатор клиента в БД.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		///     Идентификатор клиента в системе Sagi.
		/// </summary>
		public long CustomerId { get; set; }

		/// <summary>
		///     Номер телефона клиента.
		/// </summary>
		public string PhoneNumber { get; set; }

		/// <summary>
		///     Количество бонусов.
		/// </summary>
		public decimal Balance { get; set; }

		/// <summary>
		///     Идентификатор Award.
		/// </summary>
		public string AwardId { get; set; }

		/// <summary>
		///     Количество наград.
		/// </summary>
		public int ReceivedStampCount { get; set; }
	}
}