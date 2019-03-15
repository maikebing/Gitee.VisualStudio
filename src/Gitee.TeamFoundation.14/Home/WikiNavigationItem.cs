﻿using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using Microsoft.TeamFoundation.Controls;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Gitee.TeamFoundation.Home
{
    [TeamExplorerNavigationItem(Settings.WikiNavigationItemId, Settings.Wiki)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WikiNavigationItem : GiteeNavigationItem
    {
        private readonly ITeamExplorerServices _tes;

        [ImportingConstructor]
        public WikiNavigationItem(IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IWebService ws)
           : base(Octicon.book, git, shell, storage, tes, ws)
        {
            _tes = tes;

            Text = Strings.Items_Wiki;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            IsVisible = IsVisible && _tes.Project != null && _tes.Project.HasWiki;
        }

        protected override void SetDefaultColors()
        {
            m_defaultArgbColorBrush = new SolidColorBrush(Colors.BlueNavigationItem);
        }

        public override void Execute()
        {
            OpenInBrowser(_tes.Project.Url);
        }
    }
}
      