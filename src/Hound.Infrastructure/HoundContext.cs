using System.Reflection;
using Hound.Domain.Guild;
using Microsoft.EntityFrameworkCore;

namespace Hound.Infrastructure;

internal sealed class HoundContext : DbContext
{
	public HoundContext(DbContextOptions<HoundContext> options)
		: base(options)
	{
		Database.EnsureCreated();
	}

	public DbSet<GuildSettings>? Guilds { get; set; }
	public DbSet<GuildRainbowRole>? GuildRainbowRoles { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder
			.UseIdColumnNameAtBeginning()
			.UseSnakeCaseNamingConvention();

		base.OnConfiguring(optionsBuilder);
	}
}
