using Hound.Domain.SeedWork;

namespace Hound.Domain.Guild;

public interface IGuildRainbowRoleRepository : IRepositoryBase<GuildRainbowRole>
{
	Task<IReadOnlyCollection<GuildRainbowRole>> GetAllColors();

	Task<IReadOnlyCollection<GuildRainbowRole>> GetColorsForRole(ulong roleId, bool trackChanges = false);
}