﻿using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Helpers;
using Gitee.VisualStudio.Shared.Helpers.Commands;
using Microsoft.VisualStudio.Threading;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gitee.VisualStudio.UI.ViewModels
{
    public class CreateSnippetViewModel : Validatable
    {
        private IDialog _dialog;
        private IMessenger _messenger;
        private IShellService _shell;
        private IStorage _storage;
        private IWebService _web;

        public CreateSnippetViewModel(IDialog dialog, IMessenger messenger, IShellService shell, IStorage storage, IWebService web)
        {
            _dialog = dialog;
            _messenger = messenger;
            _shell = shell;
            _storage = storage;
            _web = web;
            NeedOpen = true;
            Visibility = true;
            _createSnippetCommand = new DelegateCommand(OnCreateSnippet);
        }

        private string _title;

        [Required(ErrorMessageResourceType = typeof(Strings), AllowEmptyStrings = false, ErrorMessageResourceName = "CreateSnippetViewModel_TitleIsRequired")]
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title) || string.IsNullOrWhiteSpace(_title))
                {
                    _title = Strings.CreateSnippetNoTitle;
                }
                return _title;
            }
            set { SetProperty(ref _title, value); }
        }

        private string _filename;

        [Required(ErrorMessageResourceType = typeof(Strings), AllowEmptyStrings = false, ErrorMessageResourceName = "CreateSnippetViewModel_FileNameIsRequired")]
        public string FileName
        {
            get
            {
                return _filename;
            }
            set { SetProperty(ref _filename, value); }
        }

        private string _desc;

        public string Desc
        {
            get
            {
                return _desc;
            }
            set { SetProperty(ref _desc, value); }
        }

        private bool _needopen;

        public bool NeedOpen
        {
            get
            {
                return _needopen;
            }
            set { SetProperty(ref _needopen, value); }
        }

        private string _code;

        [Required(ErrorMessageResourceType = typeof(Strings), AllowEmptyStrings = false, ErrorMessageResourceName = "CreateSnippetViewModel_CodeIsRequired")]
        public string Code
        {
            get
            {
                return _code;
            }
            set { SetProperty(ref _code, value); }
        }

        private bool _visibility;

        [Required(ErrorMessageResourceType = typeof(Strings), AllowEmptyStrings = false, ErrorMessageResourceName = "CreateSnippetViewModel_VisibilityIsRequired")]
        public bool Visibility
        {
            get
            {
                return _visibility;
            }
            set { SetProperty(ref _visibility, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private string _busyContent;

        public string BusyContent
        {
            get { return _busyContent; }
            set { SetProperty(ref _busyContent, value); }
        }

        private DelegateCommand _createSnippetCommand;

        public ICommand CreateSnippetCommand
        {
            get { return _createSnippetCommand; }
        }

        private void OnCreateSnippet()
        {
            Validate();
            if (HasErrors)
            {
                return;
            }

            IsBusy = true;
            BusyContent = Strings.CreateingASnippetPleaswWait;
            var successed = false;
            CreateSnippetResult createSnippetResult = null;
            Task.Run(async () =>
            {
                createSnippetResult = await _web.CreateSnippetAsync(Title, FileName, Desc, Code, Visibility);
                if (createSnippetResult != null)
                {
                    successed = createSnippetResult.Success;
                    if (NeedOpen && successed)
                    {
                        _shell.OpenUrl(createSnippetResult.WebUrl);
                    }
                }
            }).ContinueWith(task =>
            {
                IsBusy = false;
                BusyContent = null;

                if (successed)
                {
                    _dialog.Close();
                }
                else if (createSnippetResult?.Success==false)
                {
                    MessageBox.Show(createSnippetResult.Message);
                }
                else
                {
                    MessageBox.Show(Strings.CreateSnippetViewModel_FailedToCreateSnippet);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
        }
    }
}