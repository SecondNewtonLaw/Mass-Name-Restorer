using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Spectre.Console;

namespace NameRestorer;

public static class MainActivity
{
    const ulong srId = 0; // YOUR SERVER ID GOES HERE!
    internal static DiscordSocketClient BotClient = new(new DiscordSocketConfig()
    {
        GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMembers | GatewayIntents.GuildIntegrations,
        LogLevel = LogSeverity.Debug // YES BOY GIMME THAT FALLING TEXT!
    });
    private static string? Token { get; } = File.ReadLines($"{Environment.CurrentDirectory}/token.IGNORE").ElementAt(0);

    internal static async Task Main()
    {
        Thread Listener = new(async () => await ListenConsole());
        AnsiConsole.MarkupLine($"[yellow underline bold]Loaded Token[/][white]:[/] [red]{Token?.FixMarkup()}[/]");
        BotClient.Log += async logInfo
        => await Task.Run(() => AnsiConsole.MarkupLine($"[yellow underline bold][[Discord.Net Library]][/] -> [grey underline italic]{logInfo.Message.FixMarkup()}[/]"));
        BotClient.SlashCommandExecuted += Handlers.HandleSlashCommandAsync;

        // Login & Startup
        await BotClient.LoginAsync(TokenType.Bot, Token, true);
        await BotClient.StartAsync();
        Listener.Start();
        await Task.Delay(-1); // Not Closing Moment.
    }

    internal static async Task ListenConsole()
    {
        CommandBuilder builder = new();
        while (true)
        {
            switch (Console.ReadLine()!)
            {
                case "buildcommand":
                    AnsiConsole.MarkupLine($"[yellow bold underline][[Command Line Interface]][/] -> [green bold underline]Building commands [[SLASH]] for [yellow underline bold]{BotClient.GetGuild(srId).Name}({srId})[/][/]");
                    await builder.BuildFor(BotClient.GetGuild(srId));
                    AnsiConsole.MarkupLine("[yellow bold underline][[Command Line Interface]][/] -> [green bold underline]Command Building Completed![/] ✅");
                    break;
            }
        }
    }
}