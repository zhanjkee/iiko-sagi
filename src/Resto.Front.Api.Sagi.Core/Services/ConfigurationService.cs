using System;
using Resto.Front.Api.Sagi.Core.Interfaces;
using Resto.Front.Api.Sagi.Core.Mappers;
using Resto.Front.Api.Sagi.DataAccess.Interfaces;
using Resto.Front.Api.Sagi.Domain.Configurations;

namespace Resto.Front.Api.Sagi.Core.Services
{
	public class ConfigurationService : IConfigurationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private bool _disposed;

		public ConfigurationService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		}

		/// <inheritdoc />
		public Configuration GetConfiguration()
		{
			var configuration = _unitOfWork.ConfigurationRepository.GetConfiguration();
			if (configuration == null) return new Configuration();

			return configuration.ToDomain();
		}

		/// <inheritdoc />
		public void AddOrUpdateConfiguration(Configuration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			_unitOfWork.ConfigurationRepository.AddConfiguration(configuration.ToEntity());
			_unitOfWork.SaveChanges();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing) _unitOfWork?.Dispose();
			_disposed = true;
		}
	}
}