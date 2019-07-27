using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukoBot.Modules.Search
{
    // Doesn't represent response entirely but this is all I need for now

    public class BingSearch
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("webPages")]
        public WebPages WebPages { get; set; }
    }

    public class WebPages
    {
        [JsonProperty("webSearchUrl")]
        public string WebSearchURL { get; set; }

        [JsonProperty("totalEstimatedMatches")]
        public int TotalEstimatedMatches { get; set; }

        [JsonProperty("value")]
        public WebPage[] Value { get; set; }
    }

    public class WebPage
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("displayUrl")]
        public string DisplayUrl { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("dateLastCrawled")]
        public DateTime DateLastCrawled { get; set; }

        [JsonProperty("cachedPageUrl")]
        public string CachedPageUrl { get; set; }
    }
}
