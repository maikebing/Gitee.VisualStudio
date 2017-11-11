﻿using Gitee.VisualStudio.Shared.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitee.VisualStudio.Shared
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("new_portrait")]
        public string Avatar { get; set; }

        [JsonProperty("private_token")]
        public string Token { get; set; }

        public bool ShouldSerializeToken()
        {
            return false;
        }
    }

    public class Project
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path_with_namespace")]
        public string Path { get; set; }

        [JsonProperty("public")]
        public bool Public { get; set; }

        [JsonProperty("owner")]
        public User Owner { get; set; }

        [JsonProperty("fork?")]
        public bool Fork { get; set; }

        [JsonProperty("issues_enabled")]
        public bool IsIssueEnabled { get; set; }

        [JsonProperty("pull_requests_enabled")]
        public bool IsPullRequestsEnabled { get; set; }

        [JsonProperty("wiki_enabled")]
        public bool IsWikiEnabled { get; set; }

        public string Url
        {
            get { return $"https://git.oschina.net/{Path}.git"; }
        }

        [JsonIgnore]
        public string LocalPath { get; set; }

        [JsonIgnore]
        public Octicon Icon
        {
            get
            {
                return Public ? Octicon.@lock
                    : Fork
                    ? Octicon.repo_forked
                    : Octicon.repo;
            }
        }
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
