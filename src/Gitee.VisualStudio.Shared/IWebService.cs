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
        public string Avatar { get; set; }

        public Session Session { get; set; }
        public string Email => Detail?.Email;
    }

    public class Project 
    {
        public  bool has_wiki =>Repo.has_wiki;
        public  string Url => Repo.Url;
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

        public bool has_issues => Repo.has_issues;
        public string issues_url => Repo.issues_url;

        public  Api.Dto.Owner Owner => Repo.Owner;

        public string pulls_url => Repo.pulls_url;

        public bool pull_requests_enabled => Repo.pull_requests_enabled;

        public string releases_url => Repo.releases_url;

        public string stargazers_url=> Repo.releases_url;
    }

    public class CreateResult
    {
        public string Message { get; set; }
        public Project Project { get; set; }
    }

    public interface IWebService
    {
        User Login(string email, string password);
        IReadOnlyList<Project> GetProjects();
        CreateResult CreateProject(string name, string description, bool isPrivate);
    }
}
