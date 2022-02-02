using Resto.Front.Api.Sagi.Domain.Awards;
using Resto.Front.Api.Sagi.Domain.Cashbacks;
using Resto.Front.Api.Sagi.Domain.Customers;

namespace Resto.Front.Api.Sagi.Domain.Branches
{
	public class BranchCustomer
	{
		/// <summary>
		///     Идентификатор филиала.
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		///     Идентификатор группы.
		/// </summary>
		public long GroupId { get; set; }

		/// <summary>
		///     Клиент.
		/// </summary>
		public Customer Customer { get; set; }

		/// <summary>
		///     Награда.
		/// </summary>
		public Award Award { get; set; }

		/// <summary>
		///     Кэшбек.
		/// </summary>
		public Cashback Cashback { get; set; }
	}
}