namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHoundQuartz(this IServiceCollection services) =>
		services.AddHostedService<QuartzHostedService>()
			// see Quartz.Extensions.DependencyInjection documentation about how to configure different configuration aspects
			.AddQuartz(q => q.UseMicrosoftDependencyInjectionJobFactory())
			// Quartz.Extensions.Hosting hosting
			.AddQuartzHostedService(options => options.WaitForJobsToComplete = true)
			.RegisterQuartzJobs();

	private static IServiceCollection RegisterQuartzJobs(this IServiceCollection services) =>
		services.AddSingleton<RainbowRoleJob>()
			.AddSingleton(new JobSchedule(typeof(RainbowRoleJob), "0/3 * * * * ?")); // run every 10 seconds.
}