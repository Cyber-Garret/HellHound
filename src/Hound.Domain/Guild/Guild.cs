namespace Hound.Domain.Guild;

[UsedImplicitly]
public class Guild
{
	public ulong Id { get; init; }
	public IReadOnlyCollection<GuildRainbowRole>? RainbowRoles { get; set; }
}
