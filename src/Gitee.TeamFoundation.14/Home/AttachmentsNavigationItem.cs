using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Controls;
using Microsoft.TeamFoundation.Controls;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Gitee.TeamFoundation.Home
{
    [TeamExplorerNavigationItem(Settings.AttachmentsNavigationItemId, Settings.Attachments)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AttachmentsNavigationItem : GiteeNavigationItem
    {
        private readonly ITeamExplorerServices _tes;

        [ImportingConstructor]
        public AttachmentsNavigationItem(IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IWebService ws)
           : base(Octicon.attachment, git, shell, storage, tes, ws)
        {
            Text = Strings.Items_Attachments;
            _tes = tes;
        }
        public override void Invalidate()
        {
            base.Invalidate();
            IsVisible = IsVisible && _tes.Project != null;
        }
        public override void Execute()
        {
            OpenInBrowser(_tes.Project.ReleasesUrl);
        }
    }
}