﻿
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Bot.Services
{
	public class CommandHandlerService
	{
		private readonly IServiceProvider service;
		private readonly IConfiguration config;
		private readonly DiscordSocketClient discord;
		private CommandService command;
		public CommandHandlerService(IServiceProvider service, IConfiguration configuration)
		{
			this.service = service;
			config = configuration;
			discord = service.GetRequiredService<DiscordSocketClient>();
			command = service.GetRequiredService<CommandService>();
		}

		public async Task ConfigureAsync()
		{
			command = new CommandService(new CommandServiceConfig
			{
				DefaultRunMode = RunMode.Async,
				CaseSensitiveCommands = false
			});

			await command.AddModulesAsync(Assembly.GetEntryAssembly(), service);
		}

		public async Task HandleCommandAsync(SocketMessage message)
		{
			// Ignore if not SocketUserMessage
			if (!(message is SocketUserMessage msg)) return;

			var context = new SocketCommandContext(discord, msg);

			var argPos = 0;
			var prefix = config["Bot:Prefix"];
			// Ignore if not mention this bot or command not start from prefix
			if (!(msg.HasMentionPrefix(discord.CurrentUser, ref argPos) || msg.HasStringPrefix(prefix, ref argPos))) return;

			//search command
			var cmdSearchResult = command.Search(context, argPos);
			//If command not found just finish Task
			if (cmdSearchResult.Commands == null) return;
			//Execute command in current discord context
			var executionTask = command.ExecuteAsync(context, argPos, service);

			await executionTask.ContinueWith(task =>
			{
				// If Success or command unknown just finish Task
				if (task.Result.IsSuccess || task.Result.Error == CommandError.UnknownCommand) return;

				context.Channel.SendMessageAsync($"{context.User.Mention} Ошибка: {task.Result.ErrorReason}");
			});
		}
	}
}
