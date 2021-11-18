using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace Hound.Editor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		HubConnection connection;
		public MainWindow()
		{
			InitializeComponent();

			connection = new HubConnectionBuilder()
				.WithUrl(AddressTextBox.Text)
				.AddMessagePackProtocol()
				.Build();
			
			connection.Closed += async (error) =>
			{
				await Task.Delay(new Random().Next(0, 5) * 1000);
				await connection.StartAsync();
			};
		}

		private async void connectButton_Click(object sender, RoutedEventArgs e)
		{

			connection.On<string, string>("ReceiveMessage", (user, message) =>
			{
				Dispatcher.Invoke(() =>
				{
					var newMessage = $"{user}: {message}";
					messagesList.Items.Add(newMessage);
				});
			});

			try
			{
				await connection.StartAsync();

				messagesList.Items.Add("Connection started");
				ConnectButton.IsEnabled = false;
			}
			catch (Exception ex)
			{
				messagesList.Items.Add(ex.Message);
			}
		}

		private async void sendButton_Click(object sender, RoutedEventArgs e)
		{
			#region snippet_ErrorHandling
			try
			{
				#region snippet_InvokeAsync
				await connection.InvokeAsync("SendMessage",
					userTextBox.Text, messageTextBox.Text);
				#endregion
			}
			catch (Exception ex)
			{
				messagesList.Items.Add(ex.Message);
			}
			#endregion
		}
	}
}
