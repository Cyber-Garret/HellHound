﻿using DSharpPlus.Entities;

namespace Hound.Bot.Modules.Admin;

[RequireGuild]
[RequireUserPermissions(Permissions.Administrator)]
public class MailModule : BaseCommandModule
{
	private readonly ILogger<MailModule> _logger;
	private readonly IHubContext<MailHub, IMailingHub> _mailHub;

	public MailModule(ILogger<MailModule> logger, IHubContext<MailHub, IMailingHub> mailHub)
	{
		_logger = logger;
		_mailHub = mailHub;
	}

	/// <summary>
	/// Discord command for mailing all user with mentioned role
	/// </summary>
	[Command("рассылка")]
	public async Task MailCommand(CommandContext context, DiscordRole role, [RemainingText] string message)
	{
		var workMessage = await context.RespondAsync(string.Format(Resources.StartMailing, role.Name));

		var successCount = 0;
		var failCount = 0;
		var embed = MailingEmbed(ref context, message);


		var users = context.Guild.Members;
		_logger.LogInformation($"{context.Guild.Name} loaded: {users.Count} users.");

		foreach (var (_, discordMember) in users)
		{
			if (!discordMember.Roles.Contains(role) && role.Name != "everyone") continue;

			try
			{
				var dm = await discordMember.CreateDmChannelAsync();

				await dm.SendMessageAsync(embed: embed);
				await _mailHub.Clients.All.SucessCount(++successCount);
			}
			catch (Exception ex)
			{
				await _mailHub.Clients.All.FailedCount(++failCount);
				await _mailHub.Clients.All.FailedUserDetails(discordMember);
				_logger.LogError(ex, "Failed send message to user {name}", discordMember.Username);
			}
		}
		await workMessage.ModifyAsync(m => m.Content = string.Format(Resources.DoneMailing, role.Name, successCount + failCount, successCount, failCount));
	}

	/// <summary>
	/// Builded embed for mailing
	/// </summary>
	/// <param name="context">Command context for getting guild name,icon and user name who trigger command</param>
	/// <param name="message">Embed main content. Support discord markdown</param>
	/// <returns>Builded and ready for sending <see cref="DiscordEmbed"/></returns>
	private static DiscordEmbed MailingEmbed(ref CommandContext context, string message) =>
		new DiscordEmbedBuilder()
			.WithTitle(string.Format(Resources.MailEmbedTitle, context.User.Username, context.Guild.Name))
			.WithColor(DiscordColor.Orange)
			.WithThumbnail(context.Guild.IconUrl)
			.WithDescription(message)
			.WithTimestamp(DateTime.Now)
			.Build();
}