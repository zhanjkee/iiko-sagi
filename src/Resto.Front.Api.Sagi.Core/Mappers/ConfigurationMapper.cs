using Resto.Front.Api.Sagi.Domain.Configurations;
using Resto.Front.Api.Sagi.Entities;

namespace Resto.Front.Api.Sagi.Core.Mappers
{
	public static class ConfigurationMapper
	{
		public static ConfigurationEntity ToEntity(this Configuration source)
		{
			if (source == null) return null;

			return new ConfigurationEntity
			{
				Id = source.Id,
				Password = source.Password,
				PhoneNumber = source.PhoneNumber,
				WebAddress = source.WebAddress
			};
		}

		public static Configuration ToDomain(this ConfigurationEntity source)
		{
			if (source == null) return null;

			return new Configuration
			{
				Id = source.Id,
				Password = source.Password,
				PhoneNumber = source.PhoneNumber,
				WebAddress = source.WebAddress
			};
		}
	}
}