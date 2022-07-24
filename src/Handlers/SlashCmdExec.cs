using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Spectre.Console;

namespace NameRestorer;

public partial class Handlers
{
    public static async Task HandleSlashCommandAsync(SocketSlashCommand cmdSket)
    {
        AnsiConsole.MarkupLine($"[green][[INFO]][/] Slash Command Executed '[green]{cmdSket.Data.Name}[/]' on guild {cmdSket.Channel.GetGuild().Name} ({cmdSket.Channel.GetGuild().Id})");

        try
        {
            switch (cmdSket.Data.Name)
            {
                case "ping":
                    await Commands.PingCommand(cmdSket);
                    break;
                case "namereset":
                    await Commands.RestoreNamesCommand(cmdSket);
                    break;
                case "nameprefix":
                    await Commands.NameSetterCommand(cmdSket);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Occured!!! \n\n-> " + ex);
        }
    }
}