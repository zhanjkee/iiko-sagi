using System;
using Newtonsoft.Json;
using Resto.Front.Api.Sagi.Domain.Orders;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.Core.Mappers
{
	public static class OrderMapper
	{
		public static OrderTransactionEntity ToEntity(this OrderTransaction source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var orderEntity = new OrderTransactionEntity
			{
				Id = source.Id,
				IikoOrderId = source.IikoOrderId,
				SagiOrderId = source.SagiOrderId,
				WriteOffSum = source.WriteOffSum,
				GiveAward = source.GiveAward,
				Amount = source.Amount,
				PaymentMethod = source.PaymentMethod,
				AddStamp = source.AddStamp,
				StampCount = source.StampCount
			};

			if (source.Detail == null) return orderEntity;
			orderEntity.TransactionDetail = JsonConvert.SerializeObject(source.Detail);
			return orderEntity;
		}

		public static OrderTransaction ToDomain(this OrderTransactionEntity source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var order = new OrderTransaction
			{
				Id = source.Id,
				IikoOrderId = source.IikoOrderId,
				SagiOrderId = source.SagiOrderId,
				WriteOffSum = source.WriteOffSum,
				GiveAward = source.GiveAward,
				Amount = source.Amount,
				PaymentMethod = source.PaymentMethod,
				AddStamp = source.AddStamp,
				StampCount = source.StampCount
			};

			if (source.TransactionDetail == null) return order;
			order.Detail = JsonConvert.DeserializeObject<OrderTransactionDetail>(source.TransactionDetail);
			return order;
		}
	}
}