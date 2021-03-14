using System;
using System.Threading.Tasks;

namespace PoliceRewiredSocialDistributorLib.Social
{
    public interface IPoster
    {
        bool Accepts(SocialNetwork network);
        Task<IPostSummary> PostAsync(Post post);
        Task InitAsync();
    }
}
