using System;
using System.Collections.Generic;
using PoliceRewiredSocialDistributorLib.Social;

namespace DistributeSocialLambda
{
    public class DistributeSocialResponse
    {
        public DistributeSocialCommand input { get; set; }
        public IDictionary<string,IPostSummary> results { get; set; }
        public double duration_secs { get; set; }
    }
}
