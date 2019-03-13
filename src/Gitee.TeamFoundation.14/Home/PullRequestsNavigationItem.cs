using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using Microsoft.TeamFoundation.Controls;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;

namespace Gitee.TeamFoundation.Home
{
    [TeamExplorerNavigationItem(Settings.PullRequestsNavigationItemId, Settings.PullRequests)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsNavigationItem : GiteeNavigationItem
    {
        private readonly ITeamExplorerServices _tes;

        [ImportingConstructor]
        public PullRequestsNavigationItem(IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IWebService ws)
           : base(Octicon.git_pull_request, git, shell, storage, tes, ws)
        {
            _tes = tes;
            Text = Strings.Items_PullRequests;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            IsVisible = IsVisible && _tes.Project != null && _tes.Project.pull_requests_enabled;
        }

        protected override void SetDefaultColors()
        {
            m_defaultArgbColorBrush = new SolidColorBrush(Colors.RedNavigationItem);
        }

        public override void Execute()
        {
            OpenInBrowser(_tes.Project.pulls_url);
        }
    }
}
