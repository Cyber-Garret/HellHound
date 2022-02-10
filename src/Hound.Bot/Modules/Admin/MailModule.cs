using Hound.Domain.Models;

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
		var successCount = 0;
		var failCount = 0;
		var embed = MailingEmbed(ref context, message);

		var guildMembers = await context.Guild.GetAllMembersAsync();

		//If mentioned everyone just take all, expect bots. Otherwise filter users by role and exclude bots.
		var users =
			role.Name == "@everyone"
			? guildMembers.Where(x =>
					x.IsBot != true)
				.ToList().AsReadOnly()
			: guildMembers.Where(x =>
					x.IsBot != true
					&& x.Roles.Contains(role))
				.ToList().AsReadOnly();

		var workMessage = await context.RespondAsync(string.Format(Resources.StartMailing, users.Count, role.Name));

		_logger.LogInformation("{guildName} loaded: {count} users.", context.Guild.Name, users.Count);

		foreach (var discordMember in users)
		{
			try
			{
				await Task.Delay(TimeSpan.FromSeconds(1));

				var dm = await discordMember.CreateDmChannelAsync();

				await dm.SendMessageAsync(embed);
				await _mailHub.Clients.All.SucessCount(++successCount);
			}
			catch (Exception ex)
			{
				var model = new UserDetails(
					$"{discordMember.Username}#{discordMember.Discriminator}",
					discordMember.Nickname,
					ex.Message);

				await _mailHub.Clients.All.FailedCount(++failCount);
				await _mailHub.Clients.All.FailedUserDetails(model);

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
