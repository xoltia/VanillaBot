using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukoBot.Modules.Search
{
    public class Owner
    {
        [JsonProperty("reputation")]
        public int Reputation { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("user_type")]
        public string UserType { get; set; }

        [JsonProperty("profile_image")]
        public string ProfileImage { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("accept_rate")]
        public int? AcceptRate { get; set; }
    }

    public class Question
    {
        [JsonProperty("tags")]
        public IList<string> Tags { get; set; }

        [JsonProperty("owner")]
        public Owner Owner { get; set; }

        [JsonProperty("is_answered")]
        public bool IsAnswered { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }

        [JsonProperty("answer_count")]
        public int AnswerCount { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("last_activity_date")]
        public int LastActivityDate { get; set; }

        [JsonProperty("creation_date")]
        public int CreationDate { get; set; }

        [JsonProperty("last_edit_date")]
        public int LastEditDate { get; set; }

        [JsonProperty("question_id")]
        public int QuestionId { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("accepted_answer_id")]
        public int? AcceptedAnswerId { get; set; }
    }

    public class StackOverflowSearch
    {

        [JsonProperty("items")]
        public IList<Question> Questions { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("quota_max")]
        public int QuotaMax { get; set; }

        [JsonProperty("quota_remaining")]
        public int QuotaRemaining { get; set; }
    }
}
