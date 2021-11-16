namespace Hound.Bot.Modules;

public class MainModule : BaseCommandModule
{
	[Command("greet")]
	public async Task GreetCommand(CommandContext ctx)
	{
		await ctx.RespondAsync("Greetings! Thank you for executing me!");
	}
}