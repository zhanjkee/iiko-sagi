using System;

namespace Resto.Front.Api.Sagi.Core.Interfaces.Exceptions
{
	[Serializable]
	public class ConfigurationNotInitializedException : Exception
	{
		public ConfigurationNotInitializedException()
			: base("Настройки SAGI не инициализированы")
		{
		}
	}
}