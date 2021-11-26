using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHoundQuartz(this IServiceCollection services) =>
		services.AddHostedService<QuartzHostedService>()
			// see Quartz.Extensions.DependencyInjection documentation about how to configure different configuration aspects
			.AddQuartz(q => q.UseMicrosoftDependencyInjectionJobFactory())
			.RegisterQuartzJobs();

	private static IServiceCollection RegisterQuartzJobs(this IServiceCollection services) =>
		services.AddTransient<RainbowRoleJob>()
			.AddTransient(serviceProvider =>
			{
				var config = serviceProvider.GetService<IConfiguration>();
				return new JobSchedule(typeof(RainbowRoleJob), config["Quartz:RainbowRoleCron"]);
			});
}