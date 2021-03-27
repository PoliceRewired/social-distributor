using System;
using System.Collections.Generic;

namespace DistributeSocialLambda.DTO
{
    public class DistributeSocialCommand
    {
        public string command { get; set; }

        public IEnumerable<string> networks { get; set; }

        public string text { get; set; }
        public string tags { get; set; }
        public string linkUrl { get; set; }
        public string imageUrl { get; set; }

        public string postsCsvUrl { get; set; }
        public string rulesCsvUrl { get; set; }
    }
}
