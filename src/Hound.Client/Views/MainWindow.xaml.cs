using System.IO;
using System.Text;
using System.Windows;

namespace Hound.Client.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private HubConnection? _connection;
		public MainWindow()
		{
			InitializeComponent();

			DisconnectButton.IsEnabled = false;
		}

		private async void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			await ConnectToHub();
		}

		private async void DisconnectButton_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				await _connection?.StopAsync()!;

				StatusBarConnection.Content = _connection.State;
				ConnectButton.IsEnabled = true;
				DisconnectButton.IsEnabled = false;
			}
			catch (Exception ex)
			{
				StatusBarConnection.Content = _connection!.State;
				StatusBarMessage.Content = ex.Message;
			}
		}

		private void SaveResult_OnClick(object sender, RoutedEventArgs e)
		{
			var userDetails = MessagesListView.Items.OfType<UserDetails>().ToList();

			if (!userDetails.Any()) return;

			var stringBuilder = new StringBuilder("Имя, Имя на сервере, Причина");
			foreach (var user in userDetails)
			{
				stringBuilder.AppendLine($"{user.Username}, {user.Nickname}, {user.Reason}");
			}

			var path = Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now:dd-MM-yy-hh-mm}.txt");
			if (userDetails != null) File.WriteAllLines(path, userDetails.Select(x => x.ToString()));
		}

		private async void StatusBarMessage_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			await Task.Delay(TimeSpan.FromSeconds(20));
			StatusBarMessage.Content = string.Empty;
		}

		private async Task ConnectToHub()
		{
			_connection = new HubConnectionBuilder()
				.WithUrl(AddressTextBox.Text)
				.Build();

			_connection.Closed += async exception =>
			{
				StatusBarConnection.Content = _connection.State;

				StatusBarMessage.Content = exception?.Message;

				await Task.Delay(new Random().Next(0, 5) * 1000);
				await _connection.StartAsync();
			};

			_connection.Reconnecting += async exception =>
			{
				StatusBarMessage.Content = exception?.Message;
				StatusBarConnection.Content = _connection.State;
			};

			_connection.Reconnected += async message =>
			{
				StatusBarMessage.Content = message;
				StatusBarConnection.Content = _connection.State;
			};

			_connection.On<UserDetails>("FailedUserDetails", (details) =>
			{
				Dispatcher.Invoke(() =>
				{
					MessagesListView.Items.Add(details);
					SaveResult.IsEnabled = true;
				});
			});

			_connection.On<int>("SucessCount", (count) =>
			{
				Dispatcher.Invoke(() =>
				{
					SuccessCount.Content = count;
				});
			});

			_connection.On<int>("FailedCount", (count) =>
			{
				Dispatcher.Invoke(() =>
				{
					FailedCount.Content = count;
				});
			});

			try
			{
				await _connection.StartAsync();

				StatusBarConnection.Content = _connection.State;
				ConnectButton.IsEnabled = false;
				DisconnectButton.IsEnabled = true;
			}
			catch (Exception ex)
			{
				StatusBarConnection.Content = _connection.State;
				StatusBarMessage.Content = ex.Message;
			}
		}
	}
}
