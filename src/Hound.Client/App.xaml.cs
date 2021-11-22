using System.Windows;

using Hound.Client.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace Hound.Client
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Gets the current <see cref="App"/> instance in use
		/// </summary>
		public new static App Current => (App)Application.Current;

		/// <summary>
		/// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
		/// </summary>
		public IServiceProvider Services { get; }

		public App()
		{
			Services = ConfigureServices();

			InitializeComponent();
		}

		/// <summary>
		/// Configures the services for the application.
		/// </summary>
		private static IServiceProvider ConfigureServices()
		{
			var services = new ServiceCollection();

			// Services
			services.AddSingleton<HubConnection>();

			// Viewmodels
			services.AddTransient<MainViewModel>();
			services.AddTransient<MailingViewModel>();
			services.AddTransient<LogsViewModel>();

			return services.BuildServiceProvider();
		}
	}
}
