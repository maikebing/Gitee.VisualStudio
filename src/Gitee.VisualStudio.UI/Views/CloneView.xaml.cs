﻿using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.UI.ViewModels;
using System;
using System.Windows.Media;

namespace Gitee.VisualStudio.UI.Views
{
    /// <summary>
    /// Interaction logic for CloneView.xaml
    /// </summary>
    public partial class CloneView : Dialog
    {
        public CloneView(IMessenger messenger, IShellService shell, IStorage storage, IWebService web)
        {
            InitializeComponent();

            var vm = new CloneViewModel(this, messenger, shell, storage, web);

            DataContext = vm;
        }
    }
}
