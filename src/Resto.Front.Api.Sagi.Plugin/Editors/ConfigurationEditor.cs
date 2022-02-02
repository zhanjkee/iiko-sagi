using System;
using System.Threading;
using Resto.Front.Api.Sagi.Plugin.Controls;

namespace Resto.Front.Api.Sagi.Plugin.Editors
{
	public sealed class ConfigurationEditor : IDisposable
	{
		private ConfigurationWindow _window;

		public ConfigurationEditor()
		{
			var windowThread = new Thread(EntryPoint);
			windowThread.SetApartmentState(ApartmentState.STA);
			windowThread.Start();
		}

		public void Dispose()
		{
			_window.Dispatcher.InvokeShutdown();
			_window.Dispatcher.Thread.Join();
		}

		private void EntryPoint()
		{
			_window = new ConfigurationWindow
			{
				Topmost = true
			};
			_window.ShowDialog();
		}
	}
}