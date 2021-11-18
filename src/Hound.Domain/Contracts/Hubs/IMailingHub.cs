// ReSharper disable once CheckNamespace
namespace Hound.Domain.Contracts;

public interface IMailingHub
{
	Task SucessCount(int count);
	Task FailedCount(int count);

	Task FailedUserDetails(DiscordMember guildMember);
}