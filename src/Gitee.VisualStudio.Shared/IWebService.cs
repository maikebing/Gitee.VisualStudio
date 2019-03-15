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
        public  string Url =>  Repo.html_url;
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
        public string GiteeHomeUrl => Repo != null ? (Repo.html_url.ToLower().EndsWith(".git") ? Repo.html_url.Remove(Repo.html_url.Length - 4) : Repo.html_url) : null;

        public bool HasIssues => Repo != null ? Repo.has_issues : false;
        public string IssuesUrl => $"{GiteeHomeUrl}/issues";

        public Api.Dto.Owner Owner => Repo?.Owner;

        public string PullsUrl => $"{GiteeHomeUrl}/pulls";

        public bool PullRequestsEnabled => Repo != null ? Repo.pull_requests_enabled : false;

        public string ReleasesUrl => $"{GiteeHomeUrl}/attach_files";

        public string StatisticsUrl => $"{GiteeHomeUrl}/graph/{Repo?.default_branch}";

        public string Description => Repo?.Description;

        public string DisplayName => Repo.human_name;

        public string WikiUrl=>$"{GiteeHomeUrl}/wikis";
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
