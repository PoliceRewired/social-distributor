using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PoliceRewiredSocialDistributorLib.Social.Summary;

namespace PoliceRewiredSocialDistributorLib.Social.Posters
{
    public class DiscordPoster : IPoster
    {
        private string token;
        private DiscordSocketClient client;
        private bool ready = false;
        private SemaphoreSlim readySemaphore = new SemaphoreSlim(0, 1);

        public DiscordPoster(string token)
        {
            this.token = token;
            this.client = new DiscordSocketClient();
            this.client.Log += Log;
        }

        public bool Accepts(SocialNetwork network)
        {
            return network == SocialNetwork.discord;
        }

        public async Task InitAsync()
        {
            Console.WriteLine("Initialising Discord");
            client.Ready += Client_Ready;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
        }

        private async Task Client_Ready()
        {
            Console.WriteLine("Discord ready");
            ready = true;
            readySemaphore.Release();
        }

        public async Task<IPostSummary> PostAsync(Post post)
        {
            Console.WriteLine("Waiting to post to Discord");
            if (!ready) { await readySemaphore.WaitAsync(); }

            var guild = client.GetGuild(post.ServerId);
            Console.WriteLine("Server: " + post.ServerId + " = " + guild.Name);

            var result =
                await client
                    .GetGuild(post.ServerId)
                    .TextChannels.Single(c => c.Name.ToLower() == post.Channel.ToLower())
                    .SendMessageAsync(post.Message);

            await client.StopAsync();

            return new DiscordPostSummary(post, result);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
