using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitee.Api.Dto
{
    public class Gists
    {
        /// <summary>
        ///
        /// </summary>
        public string url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string forks_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string commits_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string description { get; set; }

        [Newtonsoft.Json.JsonProperty("public")]
        public string ispublic { get; set; }

        /// </summary>
        public string html_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int comments { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string comments_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string git_pull_url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string git_push_url { get; set; }

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
        public string forks { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string history { get; set; }
    }
}