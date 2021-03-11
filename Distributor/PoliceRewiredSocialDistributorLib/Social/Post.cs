using System;
namespace PoliceRewiredSocialDistributorLib.Social
{
    public class Post
    {
        public DateTime Created { get; private set; }

        public string Message { get; private set; }

        public Post(string message)
        {
            Created = DateTime.Now;
            Message = message;
        }

        public static Post FromText(string text)
        {
            return new Post(text);
        }
    }
}
