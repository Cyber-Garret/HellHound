
using Microsoft.Extensions.Logging;

namespace Hound.Quartz.Jobs;

[DisallowConcurrentExecution]
[PersistJobDataAfterExecution]
internal class RainbowRoleJob : IJob
{
	private readonly ILogger<RainbowRoleJob> _logger;
	private readonly IRepositoryWrapper _repository;
	private readonly DiscordClient _client;


	public RainbowRoleJob(ILogger<RainbowRoleJob> logger, IRepositoryWrapper repository, DiscordClient client)
	{
		_logger = logger;
		_repository = repository;
		_client = client;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var allColors = await _repository.RainbowRole.GetAllColors();
		var rainbowRoles = allColors.GroupBy(x => x.RoleId);
		foreach (var rainbowRole in rainbowRoles)
		{
			if (rainbowRole.Count() <= 1) continue;

			var (_, guild) = _client.Guilds.First();

			var role = guild.GetRole(rainbowRole.Key);

			var newColor = rainbowRole
				.Select(x => x.Color.UIntToDiscordColor())
				.OrderBy(_ => Guid.NewGuid())
				.First();

			_logger.LogDebug("For {roleName} trying change color {oldColor} to {newColor}", role.Name, role.Color, newColor);

			await role.ModifyAsync(color: newColor);
		}
	}
}