using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.Interfaces
{
	/// <summary>
	///     Репозитории настроек.
	/// </summary>
	public interface IConfigurationRepository
	{
		/// <summary>
		///     Получить настройки плагина.
		/// </summary>
		ConfigurationEntity GetConfiguration();

		/// <summary>
		///     Добавить настройки плагина.
		/// </summary>
		void AddConfiguration(ConfigurationEntity configuration);
	}
}