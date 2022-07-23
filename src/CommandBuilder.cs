using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace NameRestorer;

public struct CommandBuilder
{
    /// <summary>
    /// Build all bot commands for a specific Guild (Discord Server)
    /// </summary>
    /// <param name="guild">Socket of the Guild.</param>
    public async Task BuildFor(SocketGuild guild)
        => await guild.BulkOverwriteApplicationCommandAsync(this.BuildCommands().ToArray());
    /// <summary>
    /// Build all bot commands for the whole bot | Takes two hours to apply between iterations.
    /// </summary>
    /// <param name="client">Bot Client.</param>
    public async Task BuildApp(DiscordSocketClient client)
        => await client.BulkOverwriteGlobalApplicationCommandsAsync(BuildCommands().ToArray());

    private List<SlashCommandProperties> BuildCommands()
    {
        List<SlashCommandProperties> commands = new();

        SlashCommandBuilder pingCommand = new()
        {
            Name = "ping",
            Description = "Get the last heartbeat from the Gateway to our bot"
        };
        SlashCommandBuilder nameRestorerCommand = new()
        {
            Name = "namereset",
            Description = "Resets user nicknames based on regular patters or text."
        };

        nameRestorerCommand.WithDefaultMemberPermissions(GuildPermission.ManageNicknames); // Make it require Nickname Management permission for it to work
        nameRestorerCommand.AddOption("pattern", ApplicationCommandOptionType.String, "The pattern you want to use.", true);
        nameRestorerCommand.AddOption("useregex", ApplicationCommandOptionType.Boolean, "Use Regex or Contains-Based checker", true);
        nameRestorerCommand.AddOption("casesensitive", ApplicationCommandOptionType.Boolean, "Should the checker be case-sensitive", true);

        commands.Add(pingCommand.Build());
        commands.Add(nameRestorerCommand.Build());
        return commands;
    }
}
