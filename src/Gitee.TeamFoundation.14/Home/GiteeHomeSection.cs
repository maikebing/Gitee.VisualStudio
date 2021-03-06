﻿using Gitee.TeamFoundation.Views;
using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Helpers;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using Microsoft.VisualStudio.Threading;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gitee.TeamFoundation.Home
{
    [TeamExplorerSection(Settings.HomeSectionId, TeamExplorerPageIds.Home, Settings.HomeSectionPriority)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GiteeHomeSection : TeamExplorerSectionBase
    {
        private readonly ITeamExplorerServices _tes;

        [ImportingConstructor]
        public GiteeHomeSection(ITeamExplorerServices tes)
        {
            _tes = tes;
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            var gitExt = ServiceProvider.GetService<IGitExt>();
            gitExt.PropertyChanged += GitExt_PropertyChanged;
        }

        private void GitExt_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveRepositories")
            {
                Task.Run(async () =>
                {
                    var result = _tes.IsGiteeRepo();
                    await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    IsVisible = result;
                }).Forget();
            }
        }

        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            var temp = new TeamExplorerSectionViewModelBase();
            temp.Title = Strings.Name;

            return temp;
        }
        public override void Refresh()
        {
            base.Refresh();
            Task.Run(async () =>
            {
                var result = _tes.IsGiteeRepo();
                await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                IsVisible = result;
            }).Forget();
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new TextBlock
            {
                Text = Strings.Description,
                TextWrapping = System.Windows.TextWrapping.Wrap
            };
        }
    }
}