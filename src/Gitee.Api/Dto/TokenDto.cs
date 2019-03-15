using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitee.Api.Dto
{
    public class TokenDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expires_in { get; set; }

        public DateTime Expires => DateTime.Now.AddSeconds(expires_in);
        /// <summary>
        /// 
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string scope { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int created_at { get; set; }
    }
}
