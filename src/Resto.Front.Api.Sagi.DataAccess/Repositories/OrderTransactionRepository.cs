using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Resto.Front.Api.Sagi.DataAccess.Interfaces;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.Repositories
{
	internal class OrderTransactionRepository : IOrderTransactionRepository
	{
		private readonly DbContext _context;

		internal OrderTransactionRepository(DbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		private DbSet<OrderTransactionEntity> OrderTransactions => _context.Set<OrderTransactionEntity>();

		/// <inheritdoc />
		public OrderTransactionEntity GetOrderTransactionByIikoOrderId(string iikoOrderId)
		{
			if (iikoOrderId == null) throw new ArgumentNullException(nameof(iikoOrderId));
			return OrderTransactions.OrderBy(x => x.CreatedAtUtc).FirstOrDefault(x => x.IikoOrderId == iikoOrderId);
		}

		/// <inheritdoc />
		public void CreateOrderTransaction(OrderTransactionEntity order)
		{
			if (order == null) throw new ArgumentNullException(nameof(order));
			OrderTransactions.Add(order);
		}

		/// <inheritdoc />
		public void UpdateOrderTransaction(OrderTransactionEntity order)
		{
			if (order == null) throw new ArgumentNullException(nameof(order));

			var orderEntity = OrderTransactions.Find(order.Id);
			if (orderEntity == null)
			{
				OrderTransactions.Add(order);
			}
			else
			{
				order.Id = orderEntity.Id;
				_context.Entry(orderEntity).CurrentValues.SetValues(order);
			}
		}

		/// <inheritdoc />
		public void DeleteOrderTransaction(int id)
		{
			var orderTransactionEntity = OrderTransactions.Find(id);
			if (orderTransactionEntity == null) return;
			OrderTransactions.Remove(orderTransactionEntity);
		}
	}
}