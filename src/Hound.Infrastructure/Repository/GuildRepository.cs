using DSharpPlus.Entities;

using Hound.Domain.Guild;

using Microsoft.EntityFrameworkCore;

namespace Hound.Infrastructure.Repository;

internal class GuildRepository : RepositoryBase<Guild>, IGuildRepository
{
	public GuildRepository(HoundContext repositoryContext)
		: base(repositoryContext)
	{
	}

	public async Task<Guild> GetGuild(ulong guildId)
	{
		//Try find entity in database
		var guild = await FindByCondition(x => x.Id == guildId)
			.FirstOrDefaultAsync();

		// if found return entity
		if (guild != null) return guild;

		// if not found, initialize, add to database and return entity
		guild = new Guild { Id = guildId };
		await Create(guild);
		return guild;
	}

	public async Task<Guild> GetGuild(DiscordGuild guild) =>
		await GetGuild(guild.Id);
}