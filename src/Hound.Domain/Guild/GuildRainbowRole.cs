namespace Hound.Domain.Guild;

[UsedImplicitly]
public class GuildRainbowRole
{
	public int Id { get; set; }
	public ulong RoleId { get; set; }
	public uint Color { get; set; }

	public ulong GuildId { get; set; }
	public Guild? Guild { get; set; }
}