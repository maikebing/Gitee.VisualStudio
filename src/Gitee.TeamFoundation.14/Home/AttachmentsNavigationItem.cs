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
        [ImportingConstructor]
        public AttachmentsNavigationItem(IGitService git, IShellService shell, IStorage storage, ITeamExplorerServices tes, IWebService ws)
           : base(Octicon.attachment, git, shell, storage, tes, ws)
        {
            Text = Strings.Items_Attachments;
        }

        protected override void SetDefaultColors()
        {
            m_defaultArgbColorBrush = new SolidColorBrush(Colors.LightBlueNavigationItem);
        }

        public override void Execute()
        {
            OpenInBrowser("attach_files");
        }
    }
}
