using System;
namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class FbPostSummary : PostSummary
    {
        private string result;

        public FbPostSummary(string result) : base()
        {
            this.result = result;
        }

        public string Result => result;
    }
}
