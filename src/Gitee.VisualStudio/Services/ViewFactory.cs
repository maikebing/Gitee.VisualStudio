using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.UI;
using Gitee.VisualStudio.UI.Views;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Gitee.VisualStudio.Services
{
    [Export(typeof(IViewFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ViewFactory : IViewFactory
    {
        [Import]
        private IGitService _git;

        [Import]
        private IMessenger _messenger;

        [Import]
        private IStorage _storage;

        [Import]
        private IShellService _shell;

        [Import]
        private IWebService _web;

        public T GetView<T>(ViewTypes type) where T: Control
        {
            T result = default(T);
            switch (type)
            {
                case ViewTypes.Clone:
                    result= new CloneView(_messenger, _shell, _storage, _web) as T;
                    break;
                case ViewTypes.Create:
                    result= new CreateView(_git, _messenger, _shell, _storage, _web) as T;
                    break;
                case ViewTypes.Login:
                    result= new LoginView(_messenger, _shell, _storage, _web) as T;
                    break;
                case ViewTypes.CreateSnippet:
                    result= new CreateSnippet(_messenger, _shell, _storage, _web) as T;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
