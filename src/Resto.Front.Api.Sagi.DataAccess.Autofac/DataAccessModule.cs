using Autofac;
using Resto.Front.Api.Sagi.DataAccess.Interfaces;
using Resto.Front.Api.Sagi.DataAccess.UoW;

namespace Resto.Front.Api.Sagi.DataAccess.Autofac
{
	public class DataAccessModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
		}
	}
}