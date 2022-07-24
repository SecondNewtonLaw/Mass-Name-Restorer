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

        SlashCommandBuilder namePrefixerCommand = new()
        {
            Name = "nameprefix",
            Description = "Prefixes user nicknames based on the role of the user."
        };
        namePrefixerCommand.AddOption("prefix", ApplicationCommandOptionType.String, "The prefix you want to add.", true);
        namePrefixerCommand.AddOption("removecurrentnick", ApplicationCommandOptionType.Boolean, "Should the bot remove the current nickname and use the username as a base", true);
        namePrefixerCommand.AddOption("targetrole", ApplicationCommandOptionType.Role, "The role you want to perform such modification to", true);

        /*
            string namePrefix = cmdCtx.Data.Options.ElementAt(0).Value.ToString()!;
            bool replaceCurrentNickName = (bool)cmdCtx.Data.Options.ElementAt(1).Value;
            ulong roleId = (ulong)cmdCtx.Data.Options.ElementAt(2).Value; // The id of the role that should be modified.
        */

        commands.Add(pingCommand.Build());
        commands.Add(nameRestorerCommand.Build());
        commands.Add(namePrefixerCommand.Build());

        return commands;
    }
}
