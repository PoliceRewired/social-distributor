using System;
using Discord.Rest;

namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class DiscordPostSummary : PostSummary
    {
        RestUserMessage result;

        public DiscordPostSummary(Post post, RestUserMessage result) : base(post)
        {
            this.result = result;
        }

        public RestUserMessage Result => result;
    }
}
