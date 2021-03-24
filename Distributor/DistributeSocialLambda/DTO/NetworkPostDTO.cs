using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PoliceRewiredSocialDistributorLib.Social;

namespace DistributeSocialLambda.DTO
{
    public class NetworkPostDTO : Dictionary<string, IPostSummary>
    {
        public NetworkPostDTO() { }
        public NetworkPostDTO(IDictionary<string, IPostSummary> dictionary) : base(dictionary) { }
        public NetworkPostDTO(IEnumerable<KeyValuePair<string, IPostSummary>> collection) : base(collection) { }
        public NetworkPostDTO(IEqualityComparer<string> comparer) : base(comparer) { }
        public NetworkPostDTO(int capacity) : base(capacity) { }
        public NetworkPostDTO(IDictionary<string, IPostSummary> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer) { }
        public NetworkPostDTO(IEnumerable<KeyValuePair<string, IPostSummary>> collection, IEqualityComparer<string> comparer) : base(collection, comparer) { }
        public NetworkPostDTO(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }
        protected NetworkPostDTO(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
