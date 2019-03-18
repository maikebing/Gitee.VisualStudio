using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using Task = System.Threading.Tasks.Task;

namespace Gitee.TeamFoundation.Home
{
    public abstract class GiteeNavigationItem : TeamExplorerNavigationItemBase
    {
        private readonly IGitService _git;
        private readonly IShellService _shell;
        private readonly IStorage _storage;
        private readonly ITeamExplorerServices _tes;
        private readonly IWebService _web;

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
            var gitExt = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService<Microsoft.VisualStudio.TeamFoundation.Git.Extensibility.IGitExt>();
            gitExt.PropertyChanged += GitExt_PropertyChanged;
        }

        private void GitExt_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveRepositories")
            {
                Task.Run(async () =>
                {
                    var isv = _tes.IsGiteeRepo();
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    IsVisible = isv;
                }).Forget();
            }
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
            IsVisible = _tes.IsGiteeRepo();
        }

        protected void OpenInBrowser(string endpoint)
        {
            _shell.OpenUrl(endpoint);
        }
    }
}