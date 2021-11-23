using Hound.Domain.Guild;

namespace Hound.Domain.Contracts;

public interface IRepositoryWrapper
{
	IGuildRepository Guild { get; }
	IGuildRainbowRoleRepository RainbowRole { get; }
	Task SaveAsync();
}