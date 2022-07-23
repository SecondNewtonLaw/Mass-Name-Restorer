using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace NameRestorer;

public static class Extensions
{
    public static SocketGuild GetGuild<T>(this T channelSocket) where T : ISocketMessageChannel
    {
        try
        {
#pragma warning disable CS8600

            if (channelSocket is not SocketGuildChannel gChan)
            {
                throw new InvalidOperationException("Channel does not point to a valid guild!");
            }

            return gChan.Guild;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occured while attempting to get the guild of channel with id -> {channelSocket.Id}. \r\n\r\nException -> {ex}");
            throw;
        }
    }
    public static SocketGuildUser GetGuildUser<T>(this T userSocket) where T : IUser
        => (userSocket as SocketGuildUser)!;
    public static async Task<List<string>> GetRoleMentions(this SocketRole[] roles)
    {
        List<string> names = new();
        await Task.Run(
        () =>
        {
            for (int i = 0; i < roles.Length; i++)
            {
                names.Add(roles[i].Mention);
            }
        });
        return names;
    }
    public static Task<bool> HasRole(this SocketGuildUser user, SocketRole role)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < user.Roles.Count; i++)
            {
                if (user.Roles.ElementAt(i).Id == role.Id)
                    return true;
            }
            return false;
        });
    }
    /// <summary>
    /// Deletes a message in an asynchrownous manner
    /// </summary>
    /// <param name="msg">The message socket</param>
    /// <param name="delay">The delay in milliseconds in which the message should be deleted.</param>
    /// <returns>a Task representing the operation.</returns>
    internal static Task MessageDeleter<T>(T msg, int delay = 5000) where T : IMessage =>
        Task.Run(async () =>
        {
            await Task.Delay(delay);
            await msg.DeleteAsync();
        });
    internal static EmbedFooterBuilder GetTimeFooter()
        => new() { Text = $"UTC Time | {DateTime.UtcNow}" };
    /// <summary>
    /// Assembles the current <see cref="char"/> enumeration into a <see cref="string"/>
    /// </summary>
    /// <param name="chars">The collcection of charaters to aseemble into a string</param>
    /// <returns>a string containing the collection of characters as a string</returns>
    internal static string AssembleToString<Type>(this Type chars) where Type : IEnumerable<char>
        => string.Join("", chars);

    /// <summary>
    /// Fixes markup syntax errors
    /// </summary>
    /// <param name="markupeddString">The string with raw markup</param>
    /// <returns>a string with it's markup fixed</returns>
    internal static string FixMarkup(this string markupedString) => markupedString.Replace("[", "[[").Replace("]", "]]");
}