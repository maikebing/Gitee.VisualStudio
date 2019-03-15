﻿using Gitee.TeamFoundation.ViewModels;
using Gitee.TeamFoundation.Views;
using Gitee.VisualStudio.Shared;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;

namespace Gitee.TeamFoundation.Sync
{
    [TeamExplorerSection(Settings.PublishSectionId, TeamExplorerPageIds.GitCommits, Settings.PublishSectionPriority)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GiteePublishSection : TeamExplorerSectionBase
    {

        private readonly IMessenger _messenger;
        private readonly IGitService _git;
        private readonly IShellService _shell;
        private readonly IStorage _storage;
        private readonly ITeamExplorerServices _tes;
        private readonly IViewFactory _viewFactory;
        private readonly IWebService _web;

        [ImportingConstructor]
        public GiteePublishSection(IMessenger messenger, IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IViewFactory viewFactory, IWebService web)
        {
            _messenger = messenger;
            _git = git;
            _shell = shell;
            _storage = storage;
            _tes = tes;
            _viewFactory = viewFactory;
            _web = web;
        }

        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            var temp = new TeamExplorerSectionViewModelBase
            {
                Title = string.Format(Strings.Publish_Title, Strings.Name)
            };

            return temp;
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            Task.Run(async () =>
            {
                return await _tes.IsGiteeRepoAsync();
            }).ContinueWith(async (Task<bool> result) =>
            {
                this.IsVisible = await result;
            });

        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new PublishSectionView();
        }

        protected override void InitializeView(SectionInitializeEventArgs e)
        {
            var view = this.SectionContent as FrameworkElement;
            if (view != null)
            {
                var temp = new PublishSectionViewModel(_messenger, _git, _shell, _storage, _tes, _viewFactory, _web);
                temp.Published += OnPublished;

                view.DataContext = temp;
            }
        }

        private void OnPublished()
        {
            IsVisible = false;
        }

        public void ShowPublish()
        {
            IsVisible = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            var disposable = ViewModel as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}