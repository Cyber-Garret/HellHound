using Hangfire;
using Hangfire.SQLite;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

Log.Information("Booting Hound Bot");

try
{
	var builder = WebApplication.CreateBuilder(args);

	//Use Serilog as default logger with configuration from appsettings.json
	builder.Host
		.UseSerilog((context, services, configuration) => configuration
			.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
			.Enrich.FromLogContext()
			.WriteTo.Console())
		.ConfigureServices((context, services) =>
		{
			// Add Hangfire services.
			services.AddHangfire(configuration => configuration
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSQLiteStorage(context.Configuration.GetConnectionString("HangfireConnection"))
				.UseSerilogLogProvider());
			// .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
			// {
			// 	CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
			// 	SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
			// 	QueuePollInterval = TimeSpan.Zero,
			// 	UseRecommendedIsolationLevel = true,
			// 	DisableGlobalLocks = true
			// }));

			// Add the processing server as IHostedService
			services.AddHangfireServer();

			services.AddSignalR();

			services.AddHostedService<BotWorker>();

			services.AddSingleton(new DiscordClient(new DiscordConfiguration
			{
				Token = context.Configuration["Discord:Token"],
				Intents = DiscordIntents.All,
				LoggerFactory = new LoggerFactory().AddSerilog()
			}));

			// Database services
			services
				.AddHoundContext(context.Configuration)
				.AddRepositoryWrapper();

			// services.AddHoundQuartz();
		});

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}

	app.UseRouting();

	app.UseEndpoints(endpoints =>
	{
		endpoints.MapHub<MailHub>("/hubs/mail");
	});

	await app.RunAsync();

	// Log message if bot correct stopped
	Log.Information("Success shutdown bot");
}
catch (Exception exception)
{
	// Log message if caught any unhandled exception
	Log.Fatal(exception, "An unhandled exception occured during bootstrapping Hound");
}
finally
{
	Log.CloseAndFlush();
}
