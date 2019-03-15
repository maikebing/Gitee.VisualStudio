using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Threading.Tasks;
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
        private Octicon octicon;

        public GiteeNavigationItem(Octicon icon, IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IWebService web)
        {
            _git = git;
            _shell = shell;
            _storage = storage;
            _tes = tes;
            _web = web;
            var brush = new SolidColorBrush(Color.FromRgb(66, 66, 66));
            brush.Freeze();
            octicon = icon;
            OnThemeChanged();
            VSColorTheme.ThemeChanged += _ =>
            {
                OnThemeChanged();
                Invalidate();
            };
        }

        private void OnThemeChanged()
        {
            var theme = Colors.DetectTheme();
            var dark = theme == "Dark";
            m_defaultArgbColorBrush = new SolidColorBrush(dark ? Colors.DarkThemeNavigationItem : Colors.LightBlueNavigationItem);
            m_icon = SharedResources.GetDrawingForIcon(octicon, dark ? Colors.DarkThemeNavigationItem : Colors.LightThemeNavigationItem, theme);
        }

        public override void Invalidate()
        {
            Task.Run(async () =>
            {
                return await _tes.IsGiteeRepoAsync() && _tes.Project != null;
            }).ContinueWith(async (Task<bool> b) =>
            {
                IsVisible = await b;
            });
        }

        protected void OpenInBrowser(string endpoint)
        {
            _shell.OpenUrl(endpoint);
        }
    }
}