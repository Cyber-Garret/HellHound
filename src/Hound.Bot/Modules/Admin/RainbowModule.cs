using System.Drawing;

using Hound.Bot.Extensions;
using Hound.Domain.Guild;

namespace Hound.Bot.Modules.Admin;

[RequireGuild]
[RequireUserPermissions(Permissions.Administrator)]
public class RainbowModule : BaseCommandModule
{
	private readonly IRepositoryWrapper _repository;

	public RainbowModule(IRepositoryWrapper repository)
	{
		_repository = repository;
	}


	public async Task ColorList(CommandContext context)
	{
		var guild = await _repository.Guild.GetGuild(context.Guild);

		if (guild.RainbowRoles == null || guild.RainbowRoles.Count == 0)
			await context.RespondAsync(Resources.ColorListEmpty);
		else
			await context.RespondAsync(ColorListEmbed(ref context, guild.RainbowRoles));
	}

	private static DiscordEmbed ColorListEmbed(ref CommandContext context, IEnumerable<GuildRainbowRole> roles)
	{
		var embed = new DiscordEmbedBuilder()
			.WithTitle(Resources.ColorListEmbedTitle)
			.WithFooter(Resources.ColorListEmbedFooterText);

		var rolesWithColors = roles.GroupBy(x => x.RoleId);

		foreach (var item in rolesWithColors)
		{
			var role = context.Guild.GetRole(item.Key);

			var colors = item.Select(color => color.Color.UIntToColor()).ToList();

			embed.AddField(string.Format(Resources.ColorListRoleFieldTitle, role.Name), string.Join(",", colors));
		}

		return embed.Build();
	}
}