﻿using Gitee.TeamFoundation.Services;
using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.Shared.Helpers;
using Gitee.VisualStudio.Shared.Helpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gitee.TeamFoundation.ViewModels
{
    public class ConnectSectionViewModel : Bindable, IDisposable
    {
        public ObservableCollection<Repository> Repositories { get; }

        private readonly IMessenger _messenger;
        private readonly IShellService _shell;
        private readonly IStorage _storage;
        private readonly ITeamExplorerServices _teamexplorer;
        private readonly IViewFactory _viewFactory;
        private readonly IWebService _web;

        public ConnectSectionViewModel(IMessenger messenger, IShellService shell, IStorage storage, ITeamExplorerServices teamexplorer, IViewFactory viewFactory, IWebService web)
        {
            messenger.Register("OnLogined", OnLogined);
            messenger.Register<string, Repository>("OnClone", OnRepositoryCloned);

            _messenger = messenger;
            _shell = shell;
            _storage = storage;
            _teamexplorer = teamexplorer;
            _viewFactory = viewFactory;
            _web = web;

            Repositories = new ObservableCollection<Repository>();

            Repositories.CollectionChanged += OnRepositoriesChanged;

            _signOutCommand = new DelegateCommand(OnSignOut);
            _cloneCommand = new DelegateCommand(OnClone);
            _createCommand = new DelegateCommand(OnCreate);
            _openRepositoryCommand = new DelegateCommand<Repository>(OnOpenRepository);
            if (_storage.IsLogined)
            {
                LoadRepositoriesAsync();
            }
        }

        public void OnLogined()
        {
            LoadRepositoriesAsync();
        }

        private void OnRepositoriesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsRepositoriesVisible));
        }

        private Repository _selectedRepository;
        public Repository SelectedRepository
        {
            get { return _selectedRepository; }
            set { SetProperty(ref _selectedRepository, value); }
        }

        private DelegateCommand _signOutCommand;
        public ICommand SignOutCommand
        {
            get { return _signOutCommand; }
        }

        private DelegateCommand _cloneCommand;
        public ICommand CloneCommand
        {
            get { return _cloneCommand; }
        }

        private DelegateCommand _createCommand;
        public ICommand CreateCommand
        {
            get { return _createCommand; }
        }

        private DelegateCommand<Repository> _openRepositoryCommand;
        public ICommand OpenRepositoryCommand
        {
            get { return _openRepositoryCommand; }
        }

        public bool IsRepositoriesVisible
        {
            get { return Repositories.Count > 0; }
        }

        private void OnSignOut()
        {
            if (MessageBox.Show(Strings.Confirm_Quit, $"{Strings.Common_Quit} {Strings.Name}", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _storage.Erase();
                _messenger.Send("OnSignOuted");
            }
        }

        private void OnClone()
        {
            var dialog = _viewFactory.GetView<Dialog>(ViewTypes.Clone);
            _shell.ShowDialog(Strings.Common_Clone, dialog);
        }

        private void OnCreate()
        {
            var dialog = _viewFactory.GetView<Dialog>(ViewTypes.Create);
            _shell.ShowDialog(Strings.Common_CreateRepository, dialog);
        }
        public void Refresh()
        {
            LoadRepositoriesAsync();
        }
        private void OnOpenRepository(Repository repo)
        {
            if (repo == null)
            {
                return;
            }

            var solution = _teamexplorer.GetSolutionPath();
            if (solution == null || !string.Equals(repo.Path, solution.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
            {
                _messenger.Send("OnOpenSolution", repo.Path);
            }
        }

        private void LoadRepositoriesAsync()
        {
            IReadOnlyList<Repository> known = null;
            IReadOnlyList<Project> remotes = null;

            Exception ex = null;
            Task.Run(async () =>
            {
                try
                {
                    remotes = await _web.GetProjectsAsync();
                    known = Registry.GetKnownRepositories();
                }
                catch (Exception e)
                {
                    ex = e;
                }
            }).ContinueWith(task =>
            {
                if (ex == null)
                {
                    Repositories.Clear();

                    var activeRepository = _teamexplorer.GetActiveRepository();

                    var valid = new List<Repository>();

                    if (known != null)
                    {
                        foreach (var k in known)
                        {
                            var r = remotes.FirstOrDefault(o => o.Name == k.Name);
                            if (r != null)
                            {
                                k.Icon = r.Icon;

                                valid.Add(k);
                            }
                        }
                    }

                    if (activeRepository != null)
                    {
                        var matched = valid.FirstOrDefault(o => string.Equals(o.Path, activeRepository.Path, StringComparison.OrdinalIgnoreCase));
                        if (matched != null)
                        {
                            matched.IsActived = true;
                        }
                    }
                    valid.Each(o => Repositories.Add(o));
                }
                else if (!(ex is UnauthorizedAccessException))
                {
                    _teamexplorer.ShowMessage(ex.Message);
                }
                else
                {
                    _teamexplorer.ShowMessage(ex.Message);
                } 


            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void OnRepositoryCloned(string url, Repository repository)
        {
            Repositories.Add(repository);
            foreach (var r in Repositories)
            {
                r.IsActived = false;
            }

            repository.IsActived = true;
        }

        public void Dispose()
        {
            _messenger.UnRegister(this);
            GC.SuppressFinalize(this);
        }
    }
}
