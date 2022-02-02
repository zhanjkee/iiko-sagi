using System;
using Autofac;
using Resto.Front.Api.Sagi.Client.Autofac;
using Resto.Front.Api.Sagi.Core.Autofac;
using Resto.Front.Api.Sagi.DataAccess.Autofac;

namespace Resto.Front.Api.Sagi.Bootstrapper
{
	public sealed class DependecyInjection
	{
		private static readonly Lazy<DependecyInjection> Lazy =
			new Lazy<DependecyInjection>(() => new DependecyInjection());

		private DependecyInjection()
		{
			Container = BuildContainer();
		}

		public static DependecyInjection Instance => Lazy.Value;

		public IContainer Container { get; }

		public static T GetService<T>()
		{
			return Lazy.Value.Container.Resolve<T>();
		}

		private static IContainer BuildContainer()
		{
			var builder = new ContainerBuilder();
			RegisterModules(builder);
			return builder.Build();
		}

		private static void RegisterModules(ContainerBuilder builder)
		{
			builder.RegisterModule<DataAccessModule>();
			builder.RegisterModule<ClientModule>();
			builder.RegisterModule<CoreModule>();
		}
	}
}