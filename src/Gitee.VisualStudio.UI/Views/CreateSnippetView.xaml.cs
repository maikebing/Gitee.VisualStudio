using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.UI.ViewModels;
using System.Windows;

namespace Gitee.VisualStudio.UI.Views
{
    /// <summary>
    /// CreateSnippet.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSnippet : Dialog
    {
        public CreateSnippet(IMessenger messenger, IShellService shell, IStorage storage, IWebService web)
        {
            InitializeComponent();
            var vm = new CreateSnippetViewModel(this, messenger, shell, storage, web);
            DataContext = vm;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}