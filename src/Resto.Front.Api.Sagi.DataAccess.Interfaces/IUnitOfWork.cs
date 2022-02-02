using System;

namespace Resto.Front.Api.Sagi.DataAccess.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		/// <summary>
		///     Репозитории транзакции заказов.
		/// </summary>
		IOrderTransactionRepository OrderTransactionRepository { get; }

		/// <summary>
		///     Репозитории настроек.
		/// </summary>
		IConfigurationRepository ConfigurationRepository { get; }

		/// <summary>
		///     Сохранить изменения.
		/// </summary>
		void SaveChanges();
	}
}