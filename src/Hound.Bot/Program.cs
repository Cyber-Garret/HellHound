Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

Log.Information("Booting Hound Bot.");

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

			services.AddHoundQuartz();
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