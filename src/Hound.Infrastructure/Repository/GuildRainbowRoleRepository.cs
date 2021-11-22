using Hound.Domain.Guild;

namespace Hound.Infrastructure.Repository;

internal class GuildRainbowRoleRepository : RepositoryBase<GuildRainbowRole>, IGuildRainbowRoleRepository
{
	public GuildRainbowRoleRepository(HoundContext repositoryContext)
		: base(repositoryContext)
	{
	}
}