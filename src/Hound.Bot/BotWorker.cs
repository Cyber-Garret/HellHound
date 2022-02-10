using System.Reflection;

using DSharpPlus.SlashCommands;

namespace Hound.Bot;

/// <summary>
/// Hosted service for correct starting and stoping Discord Client
/// </summary>
public class BotWorker : IHostedService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IConfiguration _configuration;
	private readonly DiscordClient _discordClient;
	public BotWorker(IServiceProvider serviceProvider, IConfiguration configuration, DiscordClient discordClient)
	{
		_serviceProvider = serviceProvider;
		_configuration = configuration;
		_discordClient = discordClient;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		// Command need register before connecting to discord.
		RegisterCommands();

		//Start discord client, update bot status and activity
		await _discordClient.ConnectAsync();
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		// Disconect from Discord API
		await _discordClient.DisconnectAsync();

		_discordClient.Dispose();
	}

	/// <summary>
	/// Registering traditional commands in Discord Client
	/// </summary>
	private void RegisterCommands()
	{
		var commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration
		{
			Services = _serviceProvider,
			StringPrefixes = new[] { _configuration["Discord:Prefix"] }
		});

		commands.RegisterCommands(Assembly.GetExecutingAssembly());
	}

	/// <summary>
	/// Registering slash commands in Discord Client
	/// </summary>
	private void RegisterSlashCommands()
	{
		var commands = _discordClient.UseSlashCommands(new SlashCommandsConfiguration
		{
			Services = _serviceProvider
		});
	}
}
