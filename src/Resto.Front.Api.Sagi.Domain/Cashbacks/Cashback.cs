namespace Resto.Front.Api.Sagi.Domain.Cashbacks
{
	public class Cashback
	{
		/// <summary>
		///     Процент кэшбека.
		/// </summary>
		public decimal CashbackPercentage { get; set; }

		public bool Enabled { get; set; }
	}
}