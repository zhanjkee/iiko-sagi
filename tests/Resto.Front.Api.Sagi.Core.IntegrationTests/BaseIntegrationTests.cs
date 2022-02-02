using System;
using System.Linq;
using Resto.Front.Api.Sagi.Core.Interfaces;

namespace Resto.Front.Api.Sagi.Core.IntegrationTests
{
	public abstract class BaseIntegrationTests
	{
		private static readonly Random Random = new Random();

		public const string CustomerPhoneNumber = "77476312033";
		public const string CustomerCode = "661 122";
		public const decimal Amount = 0;
		public const string PaymentMethod = "CASH";
		public static string IikoOrderId;

		protected BaseIntegrationTests()
		{
			ConfigurationService = Bootstrapper.DependecyInjection.GetService<IConfigurationService>();
			OrderTransactionService = Bootstrapper.DependecyInjection.GetService<IOrderTransactionService>();
		}

		public static IOrderTransactionService OrderTransactionService { get; private set; }
		public static IConfigurationService ConfigurationService { get; private set; }

		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[Random.Next(s.Length)]).ToArray());
		}
	}
}
