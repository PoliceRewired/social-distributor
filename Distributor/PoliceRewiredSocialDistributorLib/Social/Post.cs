using System;
using System.IO;

namespace PoliceRewiredSocialDistributorLib.Social
{
    public class Post
    {
        public DateTime Created { get; private set; }

        public Uri Image { get; private set; }
        public Uri Link { get; private set; }
        public string Tags { get; set; }
        public string Text { get; set; }

        public ulong DiscordServerId { get; private set; }
        public string DiscordChannel { get; private set; }

        public Post(string text, string tags, Uri link, Uri image)
        {
            Created = DateTime.Now;
            Tags = tags;
            Text = text;
            Image = image;
            Link = link;
        }

        public string MessageDiscord
        {
            get
            {
                return string.Format("{0}\n{1}", Text, Link.AbsoluteUri);
            }
        }

        public string MessageFacebook
        {
            get
            {
                return string.Format("{0} {1}", Text, Tags);
            }
        }

        public string MessageFacebookIncLink
        {
            get
            {
                return string.Format("{0} {1}\n{2}", Text, Tags, Link.AbsoluteUri);
            }
        }

        public string MessageTwitter
        {
            get
            {
                return string.Format("{0} {1}\n{2}", Text, Tags, Link.AbsoluteUri);
            }
        }

        public void SetDiscordChannel(ulong serverId, string channel)
        {
            DiscordServerId = serverId;
            DiscordChannel = channel;
        }
    }
}
