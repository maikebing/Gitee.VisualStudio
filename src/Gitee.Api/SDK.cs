using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using Gitee.Api.Dto;
using Newtonsoft.Json;

namespace Gitee.Api
{
    public static class SDK
    {
        public static string client_id { get; set; }
        public static string client_secret { get; set; }

        public static Task<UserDetail> GetUserAsync(this Session session) => session.Client.GetV5UserAsync(session.Token.access_token);

        /// <summary>
        ///
        /// </summary>
        /// <param name="client"></param>
        /// <see cref="https://gitee.com/api/v5/swagger#/getV5UserRepos"/>
        /// <returns></returns>
        public static Task<List<Repo>> GetReposAsync(this Session session, int page, int per_page = 20)
        {
            return Request<List<Repo>>(session, "v5/user/repos", new { sort = "full_name", page, per_page });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="session"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="isPrivate"></param>
        /// <see cref="https://gitee.com/api/v5/swagger#/postV5UserRepos"/>
        /// <returns></returns>
        public static async Task<Repo> CreateRepoAsync(this Session session, string name, string description, bool isPrivate)
        {
            return await Request<Repo>(session, "v5/user/repos", new CreateRepo() { access_token = session.Token.access_token, description = description, has_issues = true, has_wiki = true, IsPrivate = isPrivate, name = name }, Method.POST, true);
        }

        /// <summary>
        /// 此请求方法主要用来弥补自动生成的部分api无法正常使用，使用RestSharp重新实现， 在未来，可能完全取消自动生成部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">会话必要信息</param>
        /// <param name="resource">请求资源连接字符串</param>
        /// <param name="parameters">参数类， 根据<paramref name="paramAsJson"/>来决定处理为json还是query条件</param>
        /// <param name="method">Http方法，默认GET</param>
        /// <param name="paramAsJson">Gitee大部分api是作为Query传送的，只有少数是 json，因此默认为false</param>
        /// <returns></returns>
        public static async Task<T> Request<T>(this Session session, string resource, object parameters, Method method = Method.GET, bool paramAsJson = false) where T : class
        {
            T result = default(T);
            try
            {
                var request = new RestRequest(resource, method);
                if (paramAsJson)
                {
                    request.AddHeader("Content-Type", "application/json");
                    if (typeof(JObject) == parameters?.GetType())
                    {
                        request.AddParameter("undefined", parameters.ToString(), ParameterType.RequestBody);
                    }
                    else
                    {
                        request.AddJsonBody(parameters);
                    }
                }
                else
                {
                    parameters?.GetType().GetProperties().ToList().ForEach(p =>
                    {
                        request.AddQueryParameter(p.Name, Convert.ToString(p.GetValue(parameters)), true);
                    });
                    if (session.Token != null && !string.IsNullOrEmpty(session.Token.access_token) && !request.Parameters.Any(p => p.Name == "access_token"))
                    {
                        request.AddQueryParameter("access_token", session.Token.access_token);
                    }
                }
                var response = await new RestClient(session.BaseURL).ExecuteTaskAsync(request);
                if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.Accepted) && !string.IsNullOrEmpty(response.Content))
                {
                    var jtk = Newtonsoft.Json.Linq.JToken.Parse(response.Content);
                    result = jtk.ToObject<T>();
                }
                else
                {
                    var jtk = Newtonsoft.Json.Linq.JToken.Parse(response.Content);
                    var apperror = jtk.ToObject<ApiError>();
                    if (string.IsNullOrEmpty(apperror.error_description))
                    {
                        apperror.error_description = jtk["message"]?.ToString();
                    }
                    throw new GiteeApiException(apperror);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <see cref="https://gitee.com/api/v5/oauth_doc#list_2"/>
        /// <example>curl -X POST --data-urlencode "grant_type=password" --data-urlencode "username={email}" --data-urlencode "password={password}" --data-urlencode "client_id={client_id}" --data-urlencode "client_secret={client_secret}" --data-urlencode "scope=projects user_info issues notes" https://gitee.com/oauth/token</example>
        /// <returns></returns>
        public static async Task<Session> LoginAsync(string username, string password)
        {
            var _session = new Session();
            _session.Token = await _session.Request<TokenDto>("../oauth/token", new { grant_type = "password", username, password, client_id, client_secret, scope = "user_info projects pull_requests issues notes groups gists" }, Method.POST);
            _session.Client = new Client(new HttpClient());
            _session.Client.BaseUrl = _session.BaseURL;
            return _session;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="token"></param>
        /// <see cref="https://gitee.com/oauth/token?grant_type=refresh_token&refresh_token={refresh_token}"/>
        /// <returns></returns>
        public static async Task<Session> RefreshToken(string refresh_token)
        {
            var _session = new Session();
            _session.Token = await _session.Request<TokenDto>("../oauth/token", new { grant_type = "refresh_token", refresh_token }, Method.POST);
            _session.Client = new Client(new HttpClient());
            _session.Client.BaseUrl = _session.BaseURL;
            return _session;
        }

        /// <summary>
        /// 根据缓存的 token信息继续使用， 如果没过期的话
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Session ContinueByToken(string refresh_token, string access_token, DateTime exp)
        {
            var _session = new Session();
            _session.Token = new TokenDto() { access_token = access_token, refresh_token = refresh_token, expires_in = (int)exp.Subtract(new DateTime(1970, 1, 1)).TotalSeconds };
            _session.Client = new Client(new HttpClient());
            _session.Client.BaseUrl = _session.BaseURL;
            return _session;
        }

        public static Task<Gists> CreateGists(this Session session, string filename, string content, string description, bool ispublic)
        {
            var obj = new { session.Token.access_token, description, @public = ispublic.ToString().ToLower() };
            JObject jObject = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObject.Add("files", new JObject
            {
                { filename, JObject.Parse(JsonConvert.SerializeObject(new { content })) }
            });
            return Request<Gists>(session, "/v5/gists", jObject, Method.POST, true);
        }
    }
}