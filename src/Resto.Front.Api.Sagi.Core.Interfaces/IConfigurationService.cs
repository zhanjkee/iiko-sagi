using System;
using Resto.Front.Api.Sagi.Domain.Configurations;

namespace Resto.Front.Api.Sagi.Core.Interfaces
{
	public interface IConfigurationService : IDisposable
	{
		Configuration GetConfiguration();
		void AddOrUpdateConfiguration(Configuration configuration);
	}
}