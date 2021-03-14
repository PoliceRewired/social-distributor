using System;
namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class FbPostSummary : PostSummary
    {
        private string result;

        public FbPostSummary(Post post, string result) : base(post)
        {
            this.result = result;
        }

        public string Result => result;
    }
}
