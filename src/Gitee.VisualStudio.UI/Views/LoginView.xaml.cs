﻿using Gitee.VisualStudio.Shared;
using Gitee.VisualStudio.UI.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System;

namespace Gitee.VisualStudio.UI
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Dialog, IPasswordMediator
    {
        public LoginView(IMessenger messenger, IShellService shell, IStorage storage, IWebService web)
        {
            InitializeComponent();

            var vm = new LoginViewModel(this, this, messenger, shell, storage, web);

            DataContext = vm;
        }

        public string Password
        {
            get { return PasswordTextBox.Text; }
            set { PasswordTextBox.Text = value; }
        }
    }
}
