// ReSharper disable once CheckNamespace

using Hound.Domain.Models;

namespace Hound.Domain.Contracts;

public interface IMailingHub
{
	Task SucessCount(int count);
	Task FailedCount(int count);

	Task FailedUserDetails(UserDetails userDetails);
}