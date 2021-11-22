using Hound.Domain.SeedWork;

namespace Hound.Domain.Guild;

public interface IGuildRepository : IRepositoryBase<Guild>
{
	/// <summary>
	/// Get guild settings from database by id
	/// </summary>
	Task<Guild> GetGuild(ulong guildId);

	/// <summary>
	/// Get guild settings from database by <see cref="DiscordGuild"/>
	/// </summary>
	Task<Guild> GetGuild(DiscordGuild guild);
}