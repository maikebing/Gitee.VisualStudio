using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Gitee.VisualStudio.Services
{
    public enum GiteeUrlType
    {
        Master,
        CurrentBranch,
        CurrentRevision,
        CurrentRevisionFull,
        Blame,
        Commits,
        WebIDE
    }

    public sealed class GitAnalysis : IDisposable
    {
        private readonly Repository repository;
        private readonly string targetFullPath;

        public bool IsDiscoveredGitRepository => repository != null;

        public GitAnalysis(string targetFullPath)
        {
            this.targetFullPath = targetFullPath;
            var repositoryPath = LibGit2Sharp.Repository.Discover(targetFullPath);
            if (repositoryPath != null)
            {
                this.repository = new LibGit2Sharp.Repository(repositoryPath);
                RepositoryPath = repositoryPath;
            }
        }

        private static Dictionary<string, GitAnalysis> dicgit = new Dictionary<string, GitAnalysis>();

        public static GitAnalysis GetBy(string fullpath)
        {
            if (!dicgit.ContainsKey(fullpath))
            {
                dicgit.Add(fullpath, new GitAnalysis(fullpath));
            }
            return dicgit[fullpath];
        }

        public string RepositoryPath { get; private set; }
        public LibGit2Sharp.Repository Repository { get { return repository; } }

        public string GetGiteaTargetPath(GiteeUrlType urlType)
        {
            switch (urlType)
            {
                case GiteeUrlType.CurrentBranch:
                    return repository.Head.FriendlyName.Replace("origin/", "");

                case GiteeUrlType.CurrentRevision:
                    return repository.Commits.First().Id.ToString(8);

                case GiteeUrlType.CurrentRevisionFull:
                    return repository.Commits.First().Id.Sha;

                case GiteeUrlType.Master:
                default:
                    return "master";
            }
        }

        public string GetGiteaTargetDescription(GiteeUrlType urlType)
        {
            switch (urlType)
            {
                case GiteeUrlType.CurrentBranch:
                    return "浏览当前分支 " + repository.Head.FriendlyName.Replace("origin/", "");

                case GiteeUrlType.CurrentRevision:
                    return $"浏览修订 {repository.Commits.First().Id.ToString(8)}";

                case GiteeUrlType.CurrentRevisionFull:
                    return $"浏览修订 {repository.Commits.First().Id.ToString(8)} 完整 ID";

                case GiteeUrlType.Blame:
                    return "在线追溯";

                case GiteeUrlType.Commits:
                    return "浏览提交历史";

                case GiteeUrlType.Master:
                default:
                    return "浏览主分支";
            }
        }

        public string BuildGiteaUrl(GiteeUrlType urlType, Tuple<int, int> selectionLineRange)
        {
            string fileUrl = "";
            string urlRoot = GetRepoUrlRoot();

            // foo/bar.cs
            var rootDir = repository.Info.WorkingDirectory;
            var fileIndexPath = targetFullPath.Substring(rootDir.Length).Replace("\\", "/");

            var repositoryTarget = GetGiteaTargetPath(urlType);

            // line selection
            var fragment = (selectionLineRange != null)
                                ? (selectionLineRange.Item1 == selectionLineRange.Item2)
                                    ? string.Format("#L{0}", selectionLineRange.Item1)
                                    : string.Format("#L{0}-L{1}", selectionLineRange.Item1, selectionLineRange.Item2)
                                : "";

            var urlshowkind = "blob";
            if (urlType == GiteeUrlType.Blame)
            {
                urlshowkind = "blame";
            }
            if (urlType == GiteeUrlType.Commits)
            {
                urlshowkind = "commits";
            }
            if (urlType == GiteeUrlType.WebIDE)
            {
                Uri uri = new Uri(urlRoot);
                fileUrl = $"{uri.Scheme}://{uri.Host}/-/ide/project{uri.AbsolutePath}/edit/{repositoryTarget}/-/{fileIndexPath}{fragment}";
            }
            else
            {
                fileUrl = string.Format("{0}/{4}/{1}/{2}{3}", urlRoot.Trim('/'), WebUtility.UrlEncode(repositoryTarget.Trim('/')), fileIndexPath.Trim('/'), fragment, urlshowkind);
            }
            return fileUrl;
        }

        public string GetRepoUrlRoot()
        {
            string urlRoot = string.Empty;
            var originUrl = repository.Config.Get<string>("remote.origin.url");
            if (originUrl != null)
            {
                urlRoot = (originUrl.Value.EndsWith(".git", StringComparison.InvariantCultureIgnoreCase))
                  ? originUrl.Value.Substring(0, originUrl.Value.Length - 4) // remove .git
                  : originUrl.Value;
                urlRoot = Regex.Replace(urlRoot, "^git@(.+):(.+)/(.+)$", match => "http://" + string.Join("/", match.Groups.OfType<Group>().Skip(1).Select(group => group.Value)), RegexOptions.IgnoreCase);

                urlRoot = Regex.Replace(urlRoot, "(?<=^https?://)([^@/]+)@", "");
            }
            return urlRoot;
        }

        public string GetRepoOriginRemoteUrl()
        {
            string urlRoot = string.Empty;
            var originUrl = repository.Config.Get<string>("remote.origin.url");
            if (originUrl != null)
            {
                urlRoot = originUrl.Value;
            }
            return urlRoot;
        }

        private void Dispose(bool disposing)
        {
            if (repository != null)
            {
                repository.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GitAnalysis()
        {
            Dispose(false);
        }
    }
}