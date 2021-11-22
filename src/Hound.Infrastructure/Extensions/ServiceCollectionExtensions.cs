using Hound.Domain.Contracts;
using Hound.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add SQLite database context to service container with connection string from appsettings [ConnectionStrings:MainConnection].
	/// </summary>
	public static IServiceCollection AddHoundContext(this IServiceCollection services, IConfiguration config) =>
		services.AddDbContext<HoundContext>(options =>
			options.UseSqlite(config.GetConnectionString("MainConnection")));

	/// <summary>
	/// Add Repository wrapper to services for working with database from one interface <see cref="IRepositoryWrapper"/>
	/// </summary>
	public static IServiceCollection AddRepositoryWrapper(this IServiceCollection service) =>
		service.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
}