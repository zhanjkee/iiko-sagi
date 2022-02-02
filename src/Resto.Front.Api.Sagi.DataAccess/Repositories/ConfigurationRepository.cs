using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Resto.Front.Api.Sagi.DataAccess.Interfaces;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.DataAccess.Repositories
{
	internal class ConfigurationRepository : IConfigurationRepository
	{
		private readonly DbContext _context;

		internal ConfigurationRepository(DbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		private DbSet<ConfigurationEntity> Configurations => _context.Set<ConfigurationEntity>();

		/// <inheritdoc />
		public ConfigurationEntity GetConfiguration()
		{
			return Configurations.SingleOrDefault();
		}

		/// <inheritdoc />
		public void AddConfiguration(ConfigurationEntity configuration)
		{
			if (configuration == null) throw new ArgumentNullException(nameof(configuration));

			var configurationEntity = GetConfiguration();
			if (configurationEntity == null)
			{
				Configurations.Add(configuration);
			}
			else
			{
				configuration.Id = configurationEntity.Id;
				_context.Entry(configurationEntity).CurrentValues.SetValues(configuration);
			}
		}
	}
}