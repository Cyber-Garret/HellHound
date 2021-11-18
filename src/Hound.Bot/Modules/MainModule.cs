namespace Hound.Bot.Modules;

public class MainModule : BaseCommandModule
{
	[Command("пинг")]
	public async Task PingCommand(CommandContext context)
	{
		await context.RespondAsync($"Понг! Задержка равна: {context.Client.Ping} мс.");
	}
}