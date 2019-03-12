using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;

namespace Gitee.Api
{
    public static class SDK
    {

        private static TokenDto tokenDto = null;

        public static Task<UserDetail> GetUserAsync(this Client client) => client.GetV5UserAsync(tokenDto.access_token);

        public static Task<Project> GetReposAsync(this Client client) => client.GetV5UserReposAsync(tokenDto.access_token, Visibility.All, "owner, organization_member", null, Sort10.Updated, null, 0, 100);
        /*
         curl -X POST --data-urlencode "grant_type=password" --data-urlencode "username={email}" --data-urlencode "password={password}" --data-urlencode "client_id={client_id}" --data-urlencode "client_secret={client_secret}" --data-urlencode "scope=projects user_info issues notes" https://gitee.com/oauth/token

         */

        public static async Task<Client> LoginAsync(string username, string password)
        {
            Client client = null;
            var request = new RestRequest("oauth/token", Method.POST, DataFormat.Json);
            request.AddQueryParameter("grant_type", "password");
            request.AddQueryParameter("username", username, true);
            request.AddQueryParameter("password", password , true);
            request.AddQueryParameter("client_id", SdkConfig.client_id);
            request.AddQueryParameter("client_secret", SdkConfig.client_secret);
            request.AddQueryParameter("scope", "projects user_info issues notes", true);
            try
            {
                var response = await new RestClient("https://gitee.com/").ExecuteTaskAsync(request);
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                {
                    var jtk = Newtonsoft.Json.Linq.JToken.Parse(response.Content);
                    tokenDto = jtk.ToObject<TokenDto>();
                    client = new Client(new HttpClient());
                    client.BaseUrl = "https://gitee.com/api/";
                }
                else
                {
                    var jtk = Newtonsoft.Json.Linq.JToken.Parse(response.Content);
                    var apperror = jtk.ToObject<ApiError>();
                    throw new GiteeApiException(apperror);
                }
            }
            catch (Exception ex)
            {
                throw new GiteeApiException(ex.Message,ex);
            }
            return client;
        }
    }


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
