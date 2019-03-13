using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitee.Api.Dto
{
    public class CreateRepo
    {
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string homepage { get; set; }
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
        public bool auto_init { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string gitignore_template { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string license_template { get; set; }
        [Newtonsoft.Json.JsonProperty("private")]
        public bool IsPrivate { get; set; }
}
}
