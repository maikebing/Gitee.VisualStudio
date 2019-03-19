using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitee.Api.Dto
{
    public class Owner
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string login { get; set; }

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [Newtonsoft.Json.JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string html_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string followers_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string following_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string gists_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string starred_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string subscriptions_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string organizations_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string repos_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string events_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string received_events_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string site_admin { get; set; }
    }

    public class RepoPermission
    {
        /// <summary>
        ///
        /// </summary>
        public string pull { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string push { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string admin { get; set; }
    }

    public class Repo
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string full_name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string human_name { get; set; }

        [Newtonsoft.Json.JsonProperty("url")]
        public string Url { get; set; }

        [Newtonsoft.Json.JsonProperty("namespace")]
        public Namespace Namespace { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string path { get; set; }

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("owner")]
        public Owner Owner { get; set; }

        [Newtonsoft.Json.JsonProperty("description")]
        public string Description { get; set; }

        [Newtonsoft.Json.JsonProperty("private")]
        public bool IsPrivate { get; set; }

        [Newtonsoft.Json.JsonProperty("public")]
        public bool IsPublic { get; set; }

        [Newtonsoft.Json.JsonProperty("internal")]
        public bool IsInternal { get; set; }

        public bool fork { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string html_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string forks_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string keys_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string collaborators_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string hooks_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string branches_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string tags_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string blobs_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string stargazers_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string contributors_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string commits_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string comments_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string issue_comment_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string issues_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string pulls_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string milestones_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string notifications_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string labels_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string releases_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string recommend { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string homepage { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string language { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int forks_count { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int stargazers_count { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int watchers_count { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string default_branch { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int open_issues_count { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool has_issues { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool has_wiki { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool pull_requests_enabled { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool has_page { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string license { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string outsourced { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string project_creator { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> members { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string pushed_at { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string updated_at { get; set; }

        /// <summary>
        ///
        /// </summary>
       // public string parent { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string paas { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string stared { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string watched { get; set; }

        /// <summary>
        ///
        /// </summary>
        public RepoPermission permission { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string relation { get; set; }
    }
}