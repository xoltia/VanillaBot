using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
