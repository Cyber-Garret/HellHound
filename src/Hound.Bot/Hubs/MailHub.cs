namespace Hound.Bot.Hubs;

public class MailHub : Hub<IMailingHub>
{
	public async Task SendSuccessCountToClients(int count)
	{
		await Clients.All.SucessCount(count);
	}

	public async Task SendFailedCountToClients(int count)
	{
		await Clients.All.FailedCount(count);
	}

	public async Task SenfFailedMailDetailsToClient(DiscordMember guildMember)
	{
		await Clients.All.FailedUserDetails(guildMember);
	}
}