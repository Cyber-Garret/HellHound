﻿using System.Windows;
using System.Windows.Controls;

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

			DisconnectButton.IsEnabled = false;

			_connection = new HubConnectionBuilder()
				.WithUrl(AddressTextBox.Text)
				.Build();

			_connection.Closed += async (error) =>
			{
				StatusBarConnection.Content = _connection.State;

				StatusBarMessage.Content = error?.Message;

				await Task.Delay(new Random().Next(0, 5) * 1000);
				await _connection.StartAsync();
			};

			_connection.Reconnecting += async (exception) =>
			{
				StatusBarConnection.Content = _connection.State;
			};

			_connection.Reconnected += async (exception) =>
			{
				StatusBarConnection.Content = _connection.State;
			};
		}

		private async void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			_connection.On<UserDetails>("FailedUserDetails", (details) =>
			{
				Dispatcher.Invoke(() =>
				{
					MessagesListView.Items.Add(details);
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

		private async void DisconnectButton_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				await _connection.StopAsync();

				StatusBarConnection.Content = _connection.State;
				ConnectButton.IsEnabled = true;
				DisconnectButton.IsEnabled = false;
			}
			catch (Exception ex)
			{
				StatusBarConnection.Content = _connection.State;
				StatusBarMessage.Content = ex.Message;
			}
		}
	}
}
