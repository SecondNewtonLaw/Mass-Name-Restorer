using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace NameRestorer;

internal partial class Commands
{
    public static async Task NameSetterCommand(SocketSlashCommand cmdCtx)
    {
        await cmdCtx.DeferAsync();
        string namePrefix = cmdCtx.Data.Options.ElementAt(0).Value.ToString()!;
        bool replaceCurrentNickName = (bool)cmdCtx.Data.Options.ElementAt(1).Value;
        IRole role = (IRole)cmdCtx.Data.Options.ElementAt(2).Value; // The id of the role that should be modified.

        SocketGuild server = cmdCtx.Channel.GetGuild();

        RestFollowupMessage msg = await cmdCtx.FollowupAsync($"Obtaining all users in role <@&{role.Id}>.", allowedMentions: AllowedMentions.None);


        // The socket we are targeting.
        SocketRole targetRole = server.GetRole(role.Id);
        int cachedCount = targetRole.Members.Count();

        await msg.ModifyAsync(x =>
        {
            string yesno = replaceCurrentNickName ? "Yes" : "No";
            x.Content = $"Found {cachedCount} users in role <@&{role.Id}>. Modifying Nicknames to have at the start **`{namePrefix}`** | Replace Current NickName: {yesno} | ETA: {cachedCount * 500} ms ~ {cachedCount * 500 / 1000 / 60} min(s)";
        });

        int modifiedUsers = 0;
        int failedUsers = 0;
        StringBuilder nickBuilder = new();
        for (int i = 0; i < cachedCount; i++)
        {
            // User has the username with the prefix in it.
            if (targetRole.Members.ElementAt(i).Username.Split(' ')[0].Contains(namePrefix))
                continue;
            try
            {
                // User already has the prefix in it.
                if (targetRole.Members.ElementAt(i).Nickname.Split(' ')[0].Contains(namePrefix))
                    continue;
            }
            catch (NullReferenceException) // The user has no nickname. Continue Executing with another logic.
            {

            }

            try
            {
                await targetRole.Members.ElementAt(i).ModifyAsync(x =>
                {
                    if (!replaceCurrentNickName && x.Nickname.IsSpecified)
                    {
                        nickBuilder
                            .Append(namePrefix)
                            .Append(' ')
                            .Append(x.Nickname);
                    }
                    else
                    {
                        nickBuilder
                            .Append(namePrefix)
                            .Append(' ')
                            .Append(targetRole.Members.ElementAt(i).Username);
                    }

                    x.Nickname = nickBuilder.ToString();
                    modifiedUsers++;
                    nickBuilder.Clear();
                });
            }
            catch
            {
                failedUsers++;
            }
            // Avoid Rate Limit (To some extent)
            await Task.Delay(500);
        }
        await msg.ModifyAsync(x => x.Content = $"Done! Nicknames Modified for {modifiedUsers} users in role <@&{role.Id}>. | Failed: {failedUsers}");
    }
}