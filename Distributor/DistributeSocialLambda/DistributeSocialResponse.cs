using System;
using System.Collections.Generic;
using PoliceRewiredSocialDistributorLib.Social;

namespace DistributeSocialLambda
{
    public class DistributeSocialResponse
    {
        public DistributeSocialCommand input { get; set; }
        public IDictionary<string,string> results { get; set; } // TODO: change to <string,IPostSummary>
        public double duration_secs { get; set; }
    }
}
