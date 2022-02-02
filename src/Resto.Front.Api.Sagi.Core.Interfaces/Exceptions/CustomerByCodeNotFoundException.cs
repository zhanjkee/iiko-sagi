using System;

namespace Resto.Front.Api.Sagi.Core.Interfaces.Exceptions
{
	[Serializable]
	public class CustomerByCodeNotFoundException : Exception
	{
		public CustomerByCodeNotFoundException(string code)
			: base($"Клиент не найден по 6-значному коду: {code}")
		{
		}
	}
}