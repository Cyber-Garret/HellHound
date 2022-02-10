using System.Reflection;
using DSharpPlus.SlashCommands;
using Hangfire;

namespace Hound.Bot;

/// <summary>
/// Hosted service for correct starting and stoping Discord Client
/// </summary>
public class BotWorker : IHostedService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IConfiguration _configuration;
	private readonly DiscordClient _client;

	private readonly ILogger<BotWorker> _logger;

	public BotWorker(IServiceProvider serviceProvider,
		IConfiguration configuration,
		DiscordClient discordClient,
		ILogger<BotWorker> logger,
		IServiceScopeFactory scope)
	{
		_serviceProvider = serviceProvider;
		_configuration = configuration;
		_client = discordClient;

		_logger = logger;
		_scopeFactory = scope;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		// Command need register before connecting to discord.
		RegisterCommands();

		//Start discord client, update bot status and activity
		await _client.ConnectAsync();

		var jobId = "rainbow-job";
		RecurringJob.RemoveIfExists(jobId);

		RecurringJob.AddOrUpdate(
			recurringJobId: jobId,
			methodCall: () => Console.WriteLine($@"[{DateTime.Now}] Yay it's really cool job!"),
			cronExpression: "0/5 0 0 ? * * *");
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		// Disconect from Discord API
		await _client.DisconnectAsync();

		_client.Dispose();
	}

	/// <summary>
	/// Registering traditional commands in Discord Client
	/// </summary>
	private void RegisterCommands()
	{
		var commands = _client.UseCommandsNext(new CommandsNextConfiguration
		{
			Services = _serviceProvider, StringPrefixes = new[] { _configuration["Discord:Prefix"] }
		});

		commands.RegisterCommands(Assembly.GetExecutingAssembly());
	}

	/// <summary>
	/// Registering slash commands in Discord Client
	/// </summary>
	private void RegisterSlashCommands()
	{
		var commands = _client.UseSlashCommands(new SlashCommandsConfiguration { Services = _serviceProvider });
	}
}
