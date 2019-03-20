﻿using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Helpers;
using Gitee.VisualStudio.Shared.Helpers.Commands;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.Threading;

namespace Gitee.VisualStudio.UI.ViewModels
{
    public class CreateViewModel : Validatable
    {
        private readonly IDialog _dialog;
        private readonly IGitService _git;
        private readonly IMessenger _messenger;
        private readonly IShellService _shell;
        private readonly IStorage _storage;
        private readonly IWebService _web;

        public CreateViewModel(IDialog dialog, IGitService git, IMessenger messenger, IShellService shell, IStorage storage, IWebService web)
        {
            _dialog = dialog;
            _git = git;
            _messenger = messenger;
            _shell = shell;
            _storage = storage;
            _web = web;

            GitIgnores = new Dictionary<string, string>();
            Licenses = new Dictionary<string, string>();

            _path = storage.GetBaseRepositoryDirectory();

            LoadResources();

            _newCommand = new DelegateCommand(OnSave, CanSave);
            _browseCommand = new DelegateCommand(OnBrowse);
        }

        private void LoadResources()
        {
            GitIgnores.Add(string.Empty, Strings.Common_ChooseAGitIgnore);
            SelectedGitIgnore = string.Empty;
            foreach (var line in _git.GetGitIgnores())
            {
                GitIgnores.Add(line, $"{line} - .gitignore");
            }

            Licenses.Add(string.Empty, Strings.Common_ChooseALicense);
            SelectedLicense = string.Empty;
            foreach (var line in _git.GetLicenses())
            {
                Licenses.Add(line, line);
            }
        }

        private string _name;

        [Required(ErrorMessageResourceType =typeof(Strings), ErrorMessageResourceName = "CreateView_NameIsRequired")]
        [MaxLength(64, ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "Common_NameMaxTo")]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value, () =>
                {
                    _newCommand.InvalidateCanExecute();
                });
            }
        }

        private string _description;

        [MaxLength(500, ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "Common_DescriptionMaxTo")]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private bool _isPrivate;
        public bool IsPrivate
        {
            get { return _isPrivate; }
            set { SetProperty(ref _isPrivate, value); }
        }

        private string _path;

        [Required(ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "CreateView_PathIsRequired")]
        public string Path
        {
            get { return _path; }
            set
            {
                SetProperty(ref _path, value, () =>
                {
                    _newCommand.InvalidateCanExecute();
                });
            }
        }

        public IDictionary<string, string> GitIgnores { get; }

        private string _selectedGitIgnore;
        public string SelectedGitIgnore
        {
            get { return _selectedGitIgnore; }
            set { SetProperty(ref _selectedGitIgnore, value); }
        }

        public IDictionary<string, string> Licenses { get; }

        private string _selectedLicense;
        public string SelectedLicense
        {
            get { return _selectedLicense; }
            set { SetProperty(ref _selectedLicense, value); }
        }

        private DelegateCommand _browseCommand;
        public ICommand BrowseCommand
        {
            get { return _browseCommand; }
        }

        private DelegateCommand _newCommand;
        public ICommand NewCommand
        {
            get { return _newCommand; }
        }

        private void OnBrowse()
        {
            var browsed = _shell.BrowseFolder();
            if (browsed != null)
            {
                Path = browsed;
            }
        }

        private void OnSave()
        {
            CreateResult result = null;
            string error = null;
            string clonePath = null;
            IsBusy = true;

            Task.Run(async () =>
            {
                try
                {
                    result = await _web.CreateProjectAsync(Name, Description, IsPrivate);
                    if (result.Project != null)
                    {
                        clonePath = System.IO.Path.Combine(Path, result.Project.Name);

                       await  InitialCommitAsync(result.Project.Url);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }).ContinueWith(task =>
            {
                IsBusy = false;
                if (error != null)
                {
                    _dialog.Error(error);
                }
                else if (result.Message != null)
                {
                    _dialog.Error(result.Message);
                }
                else
                {
                    var repository = new Repository
                    {
                        Name = result.Project.Name,
                        Path = clonePath,
                        Icon = result.Project.Icon
                    };

                    _messenger.Send("OnClone", result.Project.Url, repository);

                    _dialog.Close();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
        }

        private async Task InitialCommitAsync(string url)
        {
            var user = await _storage.GetUserAsync();
            var password = _storage.GetPassword();
            _git.PushInitialCommit(user.Username, user.Email, password, url, SelectedGitIgnore, SelectedLicense);
        }

        private bool CanSave()
        {
            Validate();

            return !HasErrors;
        }
    }
}
