using System.Globalization;

using Hound.Domain.Guild;

namespace Hound.Bot.Modules.Admin;

[RequireGuild]
[RequireUserPermissions(Permissions.Administrator)]
[ModuleLifespan(ModuleLifespan.Transient)]
public class RainbowModule : BaseCommandModule
{
	private readonly IRepositoryWrapper _repository;

	public RainbowModule(IRepositoryWrapper repository)
	{
		_repository = repository;
	}

	[Command("список")]
	public async Task ColorList(CommandContext context)
	{
		var roles = await _repository.RainbowRole.GetAllColors();

		if (roles.Count == 0)
			await context.RespondAsync(Resources.ColorListEmpty);
		else
		{
			var message = new DiscordMessageBuilder
			{
				Embed = ColorListEmbed(ref context, roles)
			}
				.AddComponents(new DiscordButtonComponent(ButtonStyle.Danger,
					customId: "clean_rainbow_roles",
					label: "Очистить список цветов и ролей.",
					disabled: true));

			await context.RespondAsync(message);
		}
	}

	[Command("добавить")]
	public async Task AddColor(CommandContext context, DiscordRole? role = null, string? hexColor = null)
	{
		if (role == null || hexColor == null)
			await context.RespondAsync(Resources.ColorAddParametersIsNull);
		else
		{
			var parsedColor = TryParseStringToColor(hexColor, out var parsedHex);

			if (parsedColor == 0)
			{
				await context.RespondAsync(Resources.ColorAddWrongFormat);
			}
			else
			{
				var roleColors = await _repository.RainbowRole.GetColorsForRole(role.Id);

				if (roleColors.All(x => x.Color != parsedColor))
				{
					await _repository.RainbowRole.Create(new GuildRainbowRole
					{
						RoleId = role.Id,
						Color = parsedColor
					});
					await _repository.SaveAsync();
					await context.RespondAsync(string.Format(Resources.ColorAddSuccess, role.Name, parsedHex));
				}
				else
				{
					await context.RespondAsync(string.Format(Resources.ColorAddExist, parsedHex, role.Name));
				}
			}
		}
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

	private static uint TryParseStringToColor(string hexValue, out string parsedHex)
	{
		var hex = hexValue;

		if (hex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) ||
			hex.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase))
			hex = hex[2..];

		if (hex.StartsWith("#", StringComparison.CurrentCultureIgnoreCase))
			hex = hex[1..];

		parsedHex = hex;

		return uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var color)
			? color
			: 0;
	}
}