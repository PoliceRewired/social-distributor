using System;
using System.IO;

namespace PoliceRewiredSocialDistributorLib.Social
{
    public class Post
    {
        public DateTime Created { get; private set; }
        public string Message { get; private set; }
        public string ImagePath { get; private set; }
        public ulong ServerId { get; private set; }
        public string Channel { get; private set; }

        public Post(string message, string imagePath = null)
        {
            Created = DateTime.Now;
            Message = message;
            ImagePath = imagePath;

            // quick check
            if (imagePath != null && !File.Exists(imagePath))
            {
                throw new FileNotFoundException("Image file not found.", imagePath);
            }
        }

        public Post(ulong serverId, string channel, string message, string imagePath = null) : this(message, imagePath)
        {
            ServerId = serverId;
            Channel = channel;
        }

    }
}
