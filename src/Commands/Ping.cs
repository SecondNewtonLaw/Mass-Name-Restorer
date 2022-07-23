using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace NameRestorer;

internal partial class Commands
{
    public static async Task PingCommand(SocketSlashCommand cmdCtx)
    {
        await cmdCtx.DeferAsync();
        await cmdCtx.FollowupAsync(embed: new EmbedBuilder()
        {
            Title = "Pong!",
            Description = $"The host has a ping of {MainActivity.BotClient.Latency}ms to the Discord Gateway!",
            Footer = Extensions.GetTimeFooter()
        }.Build());
    }
}