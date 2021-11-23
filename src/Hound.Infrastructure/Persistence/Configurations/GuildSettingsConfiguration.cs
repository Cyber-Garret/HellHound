using Hound.Domain.Guild;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hound.Infrastructure.Persistence.Configurations;

public class GuildSettingsConfiguration : IEntityTypeConfiguration<GuildSettings>
{
	public void Configure(EntityTypeBuilder<GuildSettings> builder)
	{
		builder.ToTable("guild_settings")
			.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever();
	}
}