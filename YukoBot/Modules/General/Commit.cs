using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukoBot.Modules.General
{
    public partial class GithubCommit
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; }

        [JsonProperty("commit")]
        public Commit Commit { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; }

        [JsonProperty("comments_url")]
        public Uri CommentsUrl { get; set; }

        [JsonProperty("author")]
        public GithubUser Author { get; set; }

        [JsonProperty("committer")]
        public GithubUser Committer { get; set; }

        [JsonProperty("parents")]
        public Parent[] Parents { get; set; }
    }

    public partial class GithubUser
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; }

        [JsonProperty("avatar_url")]
        public Uri AvatarUrl { get; set; }

        [JsonProperty("gravatar_id")]
        public string GravatarId { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; }

        [JsonProperty("followers_url")]
        public Uri FollowersUrl { get; set; }

        [JsonProperty("following_url")]
        public string FollowingUrl { get; set; }

        [JsonProperty("gists_url")]
        public string GistsUrl { get; set; }

        [JsonProperty("starred_url")]
        public string StarredUrl { get; set; }

        [JsonProperty("subscriptions_url")]
        public Uri SubscriptionsUrl { get; set; }

        [JsonProperty("organizations_url")]
        public Uri OrganizationsUrl { get; set; }

        [JsonProperty("repos_url")]
        public Uri ReposUrl { get; set; }

        [JsonProperty("events_url")]
        public string EventsUrl { get; set; }

        [JsonProperty("received_events_url")]
        public Uri ReceivedEventsUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("site_admin")]
        public bool SiteAdmin { get; set; }
    }

    public partial class Commit
    {
        [JsonProperty("author")]
        public CommitAuthor Author { get; set; }

        [JsonProperty("committer")]
        public CommitAuthor Committer { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("tree")]
        public Tree Tree { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("comment_count")]
        public long CommentCount { get; set; }

        [JsonProperty("verification")]
        public Verification Verification { get; set; }
    }

    public partial class CommitAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }
    }

    public partial class Tree
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Verification
    {
        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("signature")]
        public object Signature { get; set; }

        [JsonProperty("payload")]
        public object Payload { get; set; }
    }

    public partial class Parent
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; }
    }
}
