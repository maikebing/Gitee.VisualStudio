﻿using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System;
using System.Windows.Media;

namespace Gitee.TeamFoundation.Home
{
    public abstract class GiteeNavigationItem : TeamExplorerNavigationItemBase
    {
        private readonly IGitService _git;
        private readonly IShellService _shell;
        private readonly IStorage _storage;
        private readonly ITeamExplorerServices _tes;
        private readonly IWebService _web;

        private Project _project;
        private string _branch;

        public GiteeNavigationItem(Octicon icon, IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IWebService web)
        {
            _git = git;
            _shell = shell;
            _storage = storage;
            _tes = tes;
            _web = web;

            var brush = new SolidColorBrush(Color.FromRgb(66, 66, 66));
            brush.Freeze();
            m_icon = SharedResources.GetDrawingForIcon(icon, brush);
        }

        public override void Invalidate()
        {
            IsVisible = _tes.IsGiteeRepo() && _tes.Project != null;
        }

        protected void OpenInBrowser(string endpoint)
        {
            var user = _storage.GetUser();

            var url = $"https://gitee.com/{user.Username}/{_tes.Project.Name}/{endpoint}";

            _shell.OpenUrl(url);
        }
    }
}
