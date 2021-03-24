using System;
using System.Collections.Generic;

namespace DistributeSocialLambda.DTO
{
    public class DistributeSocialCommand
    {
        public string command { get; set; }
        public IEnumerable<string> networks { get; set; }
        public string message { get; set; }
        public string postsCsvUrl { get; set; }
        public string rulesCsvUrl { get; set; }

    }
}
