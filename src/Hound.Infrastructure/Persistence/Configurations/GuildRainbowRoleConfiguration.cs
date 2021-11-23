using Hound.Domain.Guild;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hound.Infrastructure.Persistence.Configurations;

public class GuildRainbowRoleConfiguration : IEntityTypeConfiguration<GuildRainbowRole>
{
	public void Configure(EntityTypeBuilder<GuildRainbowRole> builder)
	{
		builder.ToTable("guild_rainbow_roles")
			.HasKey(x => x.Id);

		builder.Property(x => x.RoleId)
			.IsRequired();

		builder.Property(x => x.Color)
			.IsRequired();
	}
}