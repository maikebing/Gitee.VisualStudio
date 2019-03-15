using Gitee.Api;
using Gitee.VisualStudio.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Project = Gitee.VisualStudio.Shared.Project;
using User = Gitee.VisualStudio.Shared.User;

namespace Gitee.VisualStudio.Services
{
    [Export(typeof(IWebService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WebService : IWebService
    {
        [Import]
        private IStorage _storage;

        public async Task<IReadOnlyList<Project>> GetProjectsAsync()
        {
            var user = await _storage.GetUserAsync();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Not login yet");
            }

            var result = new List<Project>();

            var page = 1;
            while (true)
            {
                var projects = await user.Session.GetReposAsync(page);
                if (!projects.Any())
                {
                    break;
                }

                page++;
                projects.ForEach(rp =>
                {
                    result.Add(new Project() { Repo = rp });
                });
            }

            return result;
        }

        public User Login(string username, string password)
        {
            try
            {
                var ses = Gitee.Api.SDK.LoginAsync(username, password).GetAwaiter().GetResult();
                var userdetail = ses.GetUserAsync().GetAwaiter().GetResult();
                var user = new Shared.User() { Detail = userdetail, Session = ses, Username = username, Avatar = userdetail.Avatar_url };
                return user;
            }
            catch (WebException ex)
            {
                var res = (HttpWebResponse)ex.Response;
                var statusCode = (int)res.StatusCode;

                throw new Exception($"错误代码: {statusCode}");
            }
        }

        public async Task<CreateResult> CreateProjectAsync(string name, string description, bool isPrivate)
        {
            var user = await _storage.GetUserAsync();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Not login yet");
            }

            var result = new CreateResult();

            try
            {
                var repo = user.Session.CreateRepoAsync(name, description, isPrivate).GetAwaiter().GetResult();
                result.Project = new Project()
                {
                    Repo = repo
                };
                return result;
            }
            catch (WebException ex)
            {
                var res = (HttpWebResponse)ex.Response;
                var statusCode = (int)res.StatusCode;

                throw new Exception($"错误代码: {statusCode}");
            }
        }
    }
}