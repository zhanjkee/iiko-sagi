using System;

namespace Resto.Front.Api.Sagi.Core.Interfaces.Exceptions
{
	[Serializable]
	public class OrderNotFoundException : Exception
	{
		public OrderNotFoundException(string orderId)
			: base($"Не удалось найди заказ по идентификатору: {orderId} в базе данных iikoSagiDb.")
		{
		}
	}
}