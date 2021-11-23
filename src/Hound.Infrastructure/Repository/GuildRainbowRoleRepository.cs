using Hound.Domain.Guild;

using Microsoft.EntityFrameworkCore;

namespace Hound.Infrastructure.Repository;

internal class GuildRainbowRoleRepository : RepositoryBase<GuildRainbowRole>, IGuildRainbowRoleRepository
{
	public GuildRainbowRoleRepository(HoundContext repositoryContext)
		: base(repositoryContext)
	{
	}

	public async Task<IReadOnlyCollection<GuildRainbowRole>> GetAllColors() =>
		await FindAll().ToListAsync();

	public async Task<IReadOnlyCollection<GuildRainbowRole>> GetColorsForRole(ulong roleId, bool trackChanges = false) =>
		await FindByCondition(x => x.RoleId == roleId, trackChanges).ToListAsync();
}