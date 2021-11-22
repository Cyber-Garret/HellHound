using Hound.Domain.Contracts;
using Hound.Domain.Guild;
using Hound.Infrastructure.Repository;

namespace Hound.Infrastructure;

internal class RepositoryWrapper : IRepositoryWrapper
{
	private readonly HoundContext _context;
	private IGuildRepository? _guild;
	private IGuildRainbowRoleRepository? _guildRainbowRole;

	public IGuildRepository Guild =>
		_guild ??= new GuildRepository(_context);

	public IGuildRainbowRoleRepository RainbowRole =>
		_guildRainbowRole ??= new GuildRainbowRoleRepository(_context);

	public RepositoryWrapper(HoundContext context)
	{
		_context = context;
	}

	public async Task Save() =>
		await _context.SaveChangesAsync();
}