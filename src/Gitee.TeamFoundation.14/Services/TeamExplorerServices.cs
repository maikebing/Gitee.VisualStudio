using Gitee.TeamFoundation.Sync;
using Gitee.VisualStudio.Shared;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gitee.TeamFoundation
{
    [Export(typeof(ITeamExplorerServices))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TeamExplorerServices : ITeamExplorerServices
    {
        private readonly IServiceProvider serviceProvider;
#pragma warning disable 0649,0169

        [Import]
        private IGitService _git;

        [Import]
        private IWebService _web;

        [Import]
        private IStorage _storage;

#pragma warning restore 0649,0169

        /// <summary>
        /// This MEF export requires specific versions of TeamFoundation. ITeamExplorerNotificationManager is declared here so
        /// that instances of this type cannot be created if the TeamFoundation dlls are not available
        /// (otherwise we'll have multiple instances of ITeamExplorerServices exports, and that would be Bad(tm))
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private ITeamExplorerNotificationManager manager;

        [ImportingConstructor]
        public TeamExplorerServices([Import(typeof(Microsoft.VisualStudio.Shell.SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void ShowPublishSection()
        {
            var te = serviceProvider.TryGetService<ITeamExplorer>();
            var foo = te.NavigateToPage(new Guid(TeamExplorerPageIds.GitCommits), null);
            var publish = foo?.GetSection(new Guid(Settings.PublishSectionId)) as GiteePublishSection;
            publish?.ShowPublish();
        }

        public void ShowMessage(string message)
        {
            manager = serviceProvider.TryGetService<ITeamExplorer>() as ITeamExplorerNotificationManager;
            manager?.ShowNotification(message, NotificationType.Information, NotificationFlags.None, null, default(Guid));
        }

        public void ShowMessage(string message, ICommand command)
        {
            manager = serviceProvider.TryGetService<ITeamExplorer>() as ITeamExplorerNotificationManager;
            manager?.ShowNotification(message, NotificationType.Information, NotificationFlags.None, command, default(Guid));
        }

        public void ShowWarning(string message)
        {
            manager = serviceProvider.TryGetService<ITeamExplorer>() as ITeamExplorerNotificationManager;
            manager?.ShowNotification(message, NotificationType.Warning, NotificationFlags.None, null, default(Guid));
        }

        public void ShowError(string message)
        {
            manager = serviceProvider.TryGetService<ITeamExplorer>() as ITeamExplorerNotificationManager;
            manager?.ShowNotification(message, NotificationType.Error, NotificationFlags.None, null, default(Guid));
        }

        public void ClearNotifications()
        {
            manager = serviceProvider.TryGetService<ITeamExplorer>() as ITeamExplorerNotificationManager;
            manager?.ClearNotifications();
        }

        public RepositoryInfo GetActiveRepository()
        {
            if (serviceProvider == null)
            {
                return null;
            }

            var git = serviceProvider.GetService<IGitExt>();
            if (git == null)
            {
                throw new Exception("git is null");
            }

            if (git.ActiveRepositories == null)
            {
                throw new Exception("git.activeRepositories is null");
            }

            var repo = git.ActiveRepositories.FirstOrDefault();

            if (repo != null && repo.CurrentBranch != null && !string.IsNullOrEmpty(repo.CurrentBranch.Name))
            {
                return new RepositoryInfo
                {
                    Path = repo.RepositoryPath,
                    Branch = repo.CurrentBranch.Name
                };
            }
            return null;
        }

        public string GetSolutionPath()
        {
            if (serviceProvider == null)
            {
                return null;
            }
            var solution = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));

            string solutionDir, solutionFile, userFile;
            if (!ErrorHandler.Succeeded(solution.GetSolutionInfo(out solutionDir, out solutionFile, out userFile)))
            {
                return null;
            }

            if (solutionDir == null)
            {
                return null;
            }

            return solutionDir;
        }

        public Project Project { get; private set; }
        private static bool isloading = false;
        public bool IsOneGiteeRepo()
        {
            var repo = GetActiveRepository();
            if (repo == null)
            {
                return false;
            }
            var url = _git.GetRemote(repo.Path);
            return !string.IsNullOrEmpty(url) && url.ToLower().StartsWith("https://gitee.com");
        }
        public bool IsGiteeRepo()
        {
            var repo = GetActiveRepository();
            if (repo == null)
            {
                return false;
            }
            var path = repo.Path;
            var url = _git.GetRemote(path);
            if (Project == null || !string.Equals(Project.Url, url, StringComparison.OrdinalIgnoreCase))
            {
                if (!isloading)
                {
                    Task.Run(async () =>
                      {
                          isloading = true;

                          try
                          {
                              var projects = await _web.GetProjectsAsync();
                              foreach (var project in projects)
                              {
                                  if (string.Equals(project.Url, url, StringComparison.OrdinalIgnoreCase))
                                  {
                                      Project = project;
                                      break;
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              ShowMessage($"加载码云项目时遇到异常:{ex.Message}");
                          }
                          isloading = false;
                      }).Forget();
                }
            }
            return !string.IsNullOrEmpty(url) && url.ToLower().StartsWith("https://gitee.com");
        }
    }
}