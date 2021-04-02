using System;
using Discord.Rest;

namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class DiscordPostSummary : PostSummary
    {
        RestUserMessage result;

        public DiscordPostSummary(RestUserMessage result) : base()
        {
            this.result = result;
        }

        internal RestUserMessage Result => result;
    }
}
