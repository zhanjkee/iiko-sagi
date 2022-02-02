using Autofac;
using Resto.Front.Api.Sagi.Client.Interfaces;

namespace Resto.Front.Api.Sagi.Client.Autofac
{
	public class ClientModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SagiRestClientFactory>().As<ISagiRestClientFactory>()
				.InstancePerDependency();
		}
	}
}