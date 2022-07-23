using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Spectre.Console;

namespace NameRestorer;

internal partial class Commands
{
    public static async Task RestoreNamesCommand(SocketSlashCommand cmdCtx)
    {
        await cmdCtx.DeferAsync();
        string usernamePattern = cmdCtx.Data.Options.ElementAt(0).Value.ToString()!;
        bool useRegex = (bool)cmdCtx.Data.Options.ElementAt(1).Value;
        string caseSensitive = (bool)cmdCtx.Data.Options.ElementAt(2).Value ? "Yes" : "No";

        RestFollowupMessage msg = await cmdCtx.FollowupAsync("Obtaining Usercount and Users...");

        ISocketMessageChannel msgCnn = cmdCtx.Channel;
        SocketGuild targetGuild = cmdCtx.Channel.GetGuild();
        List<IGuildUser> users = (await targetGuild.GetUsersAsync().FlattenAsync()).ToList();

        #region Regex Checker

        if (useRegex)
        {
            usernamePattern = Regex.Escape(usernamePattern);
            try
            {
                Regex.IsMatch("INVALID-CONTENT", usernamePattern, RegexOptions.Compiled);
            }
            catch (ArgumentException invalidPattern)
            {
                await msg.ModifyAsync(x => x.Content = $"The regex pattern provided is __**not**__ valid. More information below... \n\n{invalidPattern.Message}");
                return;
            }
        }

        #endregion Regex Checker

        await msg.ModifyAsync(x =>
        {
            if (useRegex)
                x.Content = $"Obtained {users.Count} users.\n\nStarting username reset based on regex pattern -> **`{usernamePattern}`** | Case Sensitive: {caseSensitive}";
            else
                x.Content = $"Obtained {users.Count} users.\n\nStarting username reset based on if the username EXPLICITLY contains in any part of the name -> **`{usernamePattern}`** | Case Sensitive: {caseSensitive} | ETA: {users.Count * 300} ms ~ {users.Count * 300 / 1000 / 60} min(s)";
        });

        #region Case Sensitivity Configurator

        RegexOptions caseSensitivityREGEX = RegexOptions.None;
        StringComparison caseSensitivity = StringComparison.InvariantCulture;

        if (caseSensitive == "No")
        {
            caseSensitivityREGEX = RegexOptions.IgnoreCase;
            caseSensitivity = StringComparison.InvariantCultureIgnoreCase;
        }

        #endregion Case Sensitivity Configurator

        bool restoreName;
        int modifiedUsers = 0;

        for (int i = 0; i < users.Count; i++)
        {
            // Nickname and Name are the same, ignore user.
            if (users[i].Nickname is null)
                continue;

            #region Using Regex

            if (useRegex)
            {
                try
                {
                    restoreName = Regex.IsMatch(
                        users[i].Nickname,
                        usernamePattern,
                        RegexOptions.CultureInvariant | RegexOptions.Compiled | caseSensitivityREGEX
                    );
                }
                catch (ArgumentException invalidPattern)
                {
                    AnsiConsole.MarkupLine($"Failed to evaluate Regular Expression pattern with Excption -> {invalidPattern.Message}");
                }
                continue;
            }

            #endregion Using Regex

            #region Using Contains

            else
            {
                restoreName = users[i].Nickname.Contains(usernamePattern, caseSensitivity);
            }

            #endregion Using Contains

            if (!restoreName)
            {
                continue;
            }

            await users[i].ModifyAsync(x =>
            {
                x.Nickname = null;
                modifiedUsers++;
            });
            // Avoid Rate Limit (To some extent)
            await Task.Delay(300);
        }
        await msg.ModifyAsync(x => x.Content = $"Done! Names Modified for {modifiedUsers} users.");
    }
}