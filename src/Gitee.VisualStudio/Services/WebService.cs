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
                throw new UnauthorizedAccessException(Strings.NotLoginYet);
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

        public async Task<User> LoginAsync(string username, string password)
        {
            try
            {
                var ses = await Gitee.Api.SDK.LoginAsync(username, password);
                var userdetail = await ses.GetUserAsync();
                var user = new Shared.User() { Detail = userdetail, Session = ses, Username = username };
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
                throw new UnauthorizedAccessException(Strings.NotLoginYet);
            }

            var result = new CreateResult();

            try
            {
                var repo = await user.Session.CreateRepoAsync(name, description, isPrivate);
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

        public async Task<CreateSnippetResult> CreateSnippetAsync(string title, string fileName, string desc, string code, bool visibility)
        {
            CreateSnippetResult result = null;
            var user = await _storage.GetUserAsync();
            if (user == null)
            {
                result = new CreateSnippetResult
                {
                    Success = false,
                    Message = Strings.NotLoginYet
                };
            }
            else
            {

                try
                {

                    var gists = await user.Session.CreateGists(fileName, code, desc, visibility );

                    result = new CreateSnippetResult
                    {
                        WebUrl = gists.html_url,
                        Success = gists != null,
                        Message = Strings.GistsCreated
                    };

                }
                catch (Exception ex)
                {
                    result = new CreateSnippetResult()
                    {
                        Success = false,
                        Message = ex.Message
                    };
                }
            }
            return result;
        }
    }
}