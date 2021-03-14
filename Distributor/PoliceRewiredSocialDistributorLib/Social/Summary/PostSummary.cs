using System;
namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class PostSummary : IPostSummary
    {
        private Post post;
        private DateTime posted;
        private string failureReason;
        private bool success;
        private Exception exception;

        public PostSummary(Post post, bool success = true, string reason = null)
        {
            this.post = post;
            this.posted = DateTime.Now;
            this.success = success;
            this.failureReason = reason;
        }

        public PostSummary(Post post, Exception exception)
        {
            this.post = post;
            this.posted = DateTime.Now;
            this.success = false;
            this.exception = exception;
            this.failureReason = exception.Message;
        }

        public DateTime Posted => posted;

        public bool Success => success;

        public Exception Exception => exception;

        public string FailureReason => failureReason;

        public Post Request => post;

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
