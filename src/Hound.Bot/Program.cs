//Bootstrap serilog
Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

Log.Information("Booting Hound Bot.");

try
{
	// Prepare services and host.
	// Use Serilog as default logger
	var host = Host.CreateDefaultBuilder(args)
		.UseSerilog((context, services, configuration) => configuration
			.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
			.Enrich.FromLogContext()
			.WriteTo.Console())
		.ConfigureServices((context, services) =>
		{
			services.AddHostedService<BotWorker>();
			// Add single service of Discord Client
			services.AddSingleton(new DiscordClient(new DiscordConfiguration
			{
				Token = context.Configuration["Discord:Token"],
				Intents = DiscordIntents.All,
				LoggerFactory = new LoggerFactory().AddSerilog()
			}));
		})
		.Build();

	// Run host
	await host.RunAsync();

	// Log message if bot correct stoped
	Log.Information("Success shutdown bot.");
}
catch (Exception exception)
{
	// Log message if catched any unhandled exception
	Log.Fatal(exception, "An unhandled exception occured during bootstrapping Hound");
}
finally
{
	Log.CloseAndFlush();
}