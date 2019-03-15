using Gitee.VisualStudio.Shared.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gitee.Api;
namespace Gitee.VisualStudio.Shared
{
    public class User
    {

        public string Username { get; set; }

        public UserDetail Detail { get; set; }
        public string Avatar => Detail?.Avatar_url;

        public Session Session { get; set; }
        public string Email => Detail?.Email;
    }

    public class Project 
    {
        public  bool HasWiki =>Repo.has_wiki;
        public  string Url => $"{Repo.html_url}.git";
        public   string Name =>Repo.Name;

        public Gitee.Api.Dto.Repo Repo { get; set; }

       [JsonIgnore]
        public string LocalPath { get; set; }

        [JsonIgnore]
        public Octicon Icon
        {
            get
            {
                return Repo.IsPublic ? Octicon.@lock
                    : Repo.fork
                    ? Octicon.repo_forked
                    : Octicon.repo;
            }
        }

        public bool HasIssues => Repo.has_issues;
        public string IssuesUrl => $"{Repo.html_url}/issues";

        public  Api.Dto.Owner Owner => Repo.Owner;

        public string PullsUrl => $"{Repo.html_url}/pulls";

        public bool PullRequestsEnabled => Repo.pull_requests_enabled;

        public string ReleasesUrl => $"{Repo.html_url}/attach_files";

        public string StatisticsUrl=> $"{Repo.html_url}/repository/stats/{Repo.default_branch}";
    }

    public class CreateResult
    {
        public string Message { get; set; }
        public Project Project { get; set; }
    }

    public interface IWebService
    {
        User Login(string email, string password);
        Task<IReadOnlyList<Project>> GetProjectsAsync();
        Task<CreateResult> CreateProjectAsync(string name, string description, bool isPrivate);
    }
}
