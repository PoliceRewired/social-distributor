using System;
namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class PostSummary : IPostSummary
    {
        private DateTime posted;
        private string failureReason;
        private bool success;
        private Exception exception;

        public PostSummary(bool success = true, string reason = null)
        {
            this.posted = DateTime.Now;
            this.success = success;
            this.failureReason = reason;
        }

        public PostSummary(Exception exception)
        {
            this.posted = DateTime.Now;
            this.success = false;
            this.exception = exception;
            this.failureReason = exception.Message;
        }

        public PostSummary(string failure, Exception exception = null)
        {
            this.posted = DateTime.Now;
            this.success = false;
            this.exception = exception;
            this.failureReason = failure ?? exception?.Message ?? null;
        }

        public DateTime Posted => posted;

        public bool Success => success;

        public Exception Exception => exception;

        public string FailureReason => failureReason;

        public string Summarise()
        {
            if (Success)
            {
                return "success";
            }
            else
            {
                return failureReason;
            }
        }
    }
}
