using System;
namespace PoliceRewiredSocialDistributorLib.Social
{
    public interface IPostSummary
    {
        Post Request { get; }
        DateTime Posted { get; }
        bool Success { get; }
        Exception Exception { get; }
        string FailureReason { get; }

        string Summarise();
    }
}
