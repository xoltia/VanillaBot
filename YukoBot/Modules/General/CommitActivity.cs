using Newtonsoft.Json;
using System.Collections.Generic;

namespace YukoBot.Modules.General
{
    public class CommitActivity
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("week")]
        public int Week { get; set; }

        [JsonProperty("days")]
        public IList<int> Days { get; set; }
    }
}
