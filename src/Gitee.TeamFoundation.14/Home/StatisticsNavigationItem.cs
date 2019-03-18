using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using Microsoft.TeamFoundation.Controls;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;

namespace Gitee.TeamFoundation.Home
{
    [TeamExplorerNavigationItem(Settings.StatisticsNavigationItemId, Settings.Statistics)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StatisticsNavigationItem : GiteeNavigationItem
    {
        private readonly ITeamExplorerServices _tes;

        [ImportingConstructor]
        public StatisticsNavigationItem(IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IWebService ws)
           : base(Octicon.graph, git, shell, storage, tes, ws)
        {
            _tes = tes;
            Text = Strings.Items_Statistics;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            IsVisible = IsVisible && _tes.Project != null;
        }

        public override void Execute()
        {
            var repo = _tes.GetActiveRepository();
            OpenInBrowser(_tes.Project.StatisticsUrl);
        }
    }
}