using Autofac;
using Resto.Front.Api.Sagi.Core.Interfaces;
using Resto.Front.Api.Sagi.Core.Services;

namespace Resto.Front.Api.Sagi.Core.Autofac
{
	public class CoreModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<ConfigurationService>().As<IConfigurationService>()
				.InstancePerDependency();

			builder.RegisterType<OrderTransactionService>().As<IOrderTransactionService>()
				.InstancePerDependency();
		}
	}
}