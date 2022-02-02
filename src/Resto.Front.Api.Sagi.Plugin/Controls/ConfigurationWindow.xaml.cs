using System.Windows;
using Resto.Front.Api.Sagi.Bootstrapper;
using Resto.Front.Api.Sagi.Core.Interfaces;
using Resto.Front.Api.Sagi.Domain.Configurations;

namespace Resto.Front.Api.Sagi.Plugin.Controls
{
	/// <summary>
	///     Interaction logic for ConfigurationWindow.xaml
	/// </summary>
	public partial class ConfigurationWindow : Window
	{
		private readonly IConfigurationService _configurationService;

		public ConfigurationWindow()
		{
			InitializeComponent();
			ResizeMode = ResizeMode.NoResize;

			_configurationService = DependecyInjection.GetService<IConfigurationService>();
			var configuration = _configurationService.GetConfiguration();

			WebAddress.Password = configuration.WebAddress;
			Phone.Text = configuration.PhoneNumber;
			Password.Password = configuration.Password;
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			var configuration = new Configuration
			{
				WebAddress = WebAddress.Password,
				PhoneNumber = Phone.Text,
				Password = Password.Password
			};
			_configurationService.AddOrUpdateConfiguration(configuration);

			Close();
		}

		private void Cancel(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}