using System;
namespace PoliceRewiredSocialDistributorLib.Social
{
    public interface IPostSummary
    {
        public string Message { get; }
        public DateTime Posted { get; }

        public Post Request { get; }
    }
}
