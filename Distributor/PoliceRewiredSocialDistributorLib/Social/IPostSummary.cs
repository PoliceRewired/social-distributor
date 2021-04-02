using System;
namespace PoliceRewiredSocialDistributorLib.Social
{
    public interface IPostSummary
    {
        DateTime Posted { get; }
        bool Success { get; }
        string FailureReason { get; }
        string Summarise();
    }
}
