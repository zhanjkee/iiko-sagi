using System;
using Microsoft.EntityFrameworkCore;
using Resto.Front.Api.Sagi.DataAccess.Contexts;
using Resto.Front.Api.Sagi.DataAccess.Interfaces;
using Resto.Front.Api.Sagi.DataAccess.Repositories;

namespace Resto.Front.Api.Sagi.DataAccess.UoW
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly DbContext _context;
		private IConfigurationRepository _configurationRepository;
		private bool _disposed;
		private IOrderTransactionRepository _orderRepository;

		public UnitOfWork()
		{
			_context = new DataContext();
			_context.Database.Migrate();
		}

		public IOrderTransactionRepository OrderTransactionRepository => _orderRepository ??
		                                                                 (_orderRepository =
			                                                                 new OrderTransactionRepository(_context));

		public IConfigurationRepository ConfigurationRepository => _configurationRepository ??
		                                                           (_configurationRepository =
			                                                           new ConfigurationRepository(_context));

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing) _context.Dispose();
			_disposed = true;
		}
	}
}