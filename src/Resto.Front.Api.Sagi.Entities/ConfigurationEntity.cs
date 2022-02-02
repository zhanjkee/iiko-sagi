using Resto.Front.Api.Sagi.Entities.Base;

namespace Resto.Front.Api.Sagi.Entities
{
	/// <summary>
	///     Конфигурации плагина.
	/// </summary>
	public class ConfigurationEntity : BaseEntity
	{
		/// <summary>
		///     Веб-адрес API Sagi.
		/// </summary>
		public string WebAddress { get; set; }

		/// <summary>
		///     Номер телефона.
		/// </summary>
		public string PhoneNumber { get; set; }

		/// <summary>
		///     Пароль.
		/// </summary>
		public string Password { get; set; }
	}
}