using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitee.Api.Dto
{

    public class GiteeApiException : Exception
    {
        public GiteeApiException() { }
        public GiteeApiException(ApiError error) : base(error.error_description)
        {
            ApiError = error;

        }
        public GiteeApiException(ApiError error, Exception inner) : base(error.error_description, inner)
        {
            ApiError = error;
        }
        public GiteeApiException(string message, Exception inner) : base(message, inner)
        {

        }
        public ApiError ApiError { get; private set; }
    }

    public class ApiError
    {
        /// <summary>
        /// 
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// 授权方式无效，或者登录回调地址无效、过期或已被撤销
        /// </summary>
        public string error_description { get; set; }
    }
}
