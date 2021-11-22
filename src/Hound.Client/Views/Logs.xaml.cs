using System.Windows.Controls;

using Hound.Client.ViewModels;

namespace Hound.Client.Views
{
	/// <summary>
	/// Interaction logic for Logs.xaml
	/// </summary>
	public partial class Logs : Page
	{
		public Logs()
		{
			InitializeComponent();

			DataContext = App.Current.Services.GetService<LogsViewModel>();
		}
	}
}
