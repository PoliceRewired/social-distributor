using System;
using System.Collections.Generic;

namespace DistributeSocialLambda
{
    public class DistributeSocialCommand
    {
        public string command { get; set; }
        public IEnumerable<string> networks { get; set; }
        public string message { get; set; }

    }
}
