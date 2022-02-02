using System.Net;
using System.Reactive.Disposables;
using Resto.Front.Api.Attributes;
using Resto.Front.Api.Exceptions;
using Resto.Front.Api.Sagi.Plugin.Editors;

namespace Resto.Front.Api.Sagi.Plugin
{
	[PluginLicenseModuleId(21016318)]
	public sealed class SagiPaymentPlugin : IFrontPlugin
	{
		private readonly CompositeDisposable _subscriptions;

		public SagiPaymentPlugin()
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
			                                       SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			_subscriptions = new CompositeDisposable();
			var paymentSystem = new ExternalPaymentProcessor();

			_subscriptions.Add(paymentSystem);
			try
			{
				_subscriptions.Add(PluginContext.Operations.RegisterPaymentSystem(paymentSystem));
				// ReSharper disable once ObjectCreationAsStatement
				PluginContext.Operations.AddButtonToPluginsMenu(Resources.TitleSagiSettings,
					_ => new ConfigurationEditor());
			}
			catch (LicenseRestrictionException ex)
			{
				PluginContext.Log.Warn(ex.Message);
				return;
			}
			catch (PaymentSystemRegistrationException ex)
			{
				PluginContext.Log.Warn(
					$"Sagi payment system '{paymentSystem.PaymentSystemKey}': '{paymentSystem.PaymentSystemName}' wasn't registered. Reason: {ex.Message}");
				return;
			}

			PluginContext.Log.Info(
				$"Sagi payment system '{paymentSystem.PaymentSystemKey}': '{paymentSystem.PaymentSystemName}' was successfully registered on server.");
		}

		public void Dispose()
		{
			_subscriptions?.Dispose();
		}
	}
}