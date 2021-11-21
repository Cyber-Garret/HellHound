using System.Windows;

using Microsoft.Extensions.DependencyInjection;

namespace Hound.Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly HubConnection _connection;
		public MainWindow()
		{
			InitializeComponent();

			_connection = new HubConnectionBuilder()
				.WithUrl(AddressTextBox.Text)
				.AddMessagePackProtocol()
				.Build();

			_connection.Closed += async (error) =>
			{
				ConnectionStatus.Content = error?.Message;

				await Task.Delay(new Random().Next(0, 5) * 1000);
				await _connection.StartAsync();
			};
		}

		private async void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			_connection.On<DiscordMember>("SendFailedMailDetailsToClient", (guildMember) =>
			{
				Dispatcher.Invoke(() =>
				{
					MessagesListView.Items.Add(guildMember.Username);
				});
			});

			try
			{
				await _connection.StartAsync();

				ConnectionStatus.Content = "Connection started";
				ConnectButton.IsEnabled = false;
			}
			catch (Exception ex)
			{
				ConnectionStatus.Content = ex.Message;
			}
		}
	}
}
