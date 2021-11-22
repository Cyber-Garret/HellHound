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

	public DbSet<Guild>? Guilds { get; set; }
	public DbSet<GuildRainbowRole>? GuildRainbowRoles { get; set; }
}