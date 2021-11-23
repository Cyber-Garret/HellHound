using Hound.Domain.SeedWork;

namespace Hound.Domain.Guild;

public interface IGuildRepository : IRepositoryBase<GuildSettings>
{
	/// <summary>
	/// Get guild settings from database by id
	/// </summary>
	Task<GuildSettings> GetGuild(ulong guildId);

	/// <summary>
	/// Get guild settings from database by <see cref="DiscordGuild"/>
	/// </summary>
	Task<GuildSettings> GetGuild(DiscordGuild guild);
}