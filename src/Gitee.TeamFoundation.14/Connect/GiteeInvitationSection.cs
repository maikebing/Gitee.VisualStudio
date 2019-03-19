﻿using Gitee.VisualStudio.Shared;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gitee.TeamFoundation.Connect
{
    [TeamExplorerServiceInvitation(Settings.InvitationSectionId, Settings.InvitationSectionPriority)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GiteeInvitationSection : TeamExplorerServiceInvitationBase
    {
        private readonly IMessenger _messenger;
        private readonly IShellService _shell;
        private readonly IStorage _storage;
        private readonly IViewFactory _viewFactory;

        [ImportingConstructor]
        public GiteeInvitationSection(IMessenger messenger, IShellService shell, IStorage storage, IViewFactory viewFactory)
        {
            _messenger = messenger;
            _shell = shell;
            _storage = storage;
            _viewFactory = viewFactory;

            _messenger.Register("OnLogined", OnLogined);
            _messenger.Register("OnSignOuted", OnSignOuted);

            CanConnect = true;
            CanSignUp = true;
            ConnectLabel = Strings.Invitation_Connect;
            SignUpLabel = Strings.Invitation_SignUp;
            Name = Strings.Name;
            Provider = Strings.Provider;
            Description = Strings.Description;

            var assembly = Assembly.GetExecutingAssembly().GetName().Name;
            var image = new BitmapImage(new Uri($"pack://application:,,,/{assembly};component/Resources/logo.png", UriKind.Absolute)); ;

            var drawing = new DrawingGroup();
            drawing.Children.Add(new GeometryDrawing
            {
                Brush = new ImageBrush(image),
                Geometry = new RectangleGeometry(new Rect(new Size(image.Width, image.Height)))
            });

            Icon = new DrawingBrush(drawing);

            IsVisible = !storage.IsLogined;
        }

        public override void Connect()
        {
            var dialog = _viewFactory.GetView<Dialog>(ViewTypes.Login);
            _shell.ShowDialog(string.Format(Strings.Login_ConnectTo, Strings.Name), dialog);
        }

        public override void SignUp()
        {
            _shell.OpenUrl(@"https://gitee.com/signup");
        }

        public void OnLogined()
        {
            IsVisible = false;
        }

        public void OnSignOuted()
        {
            IsVisible = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            _messenger.UnRegister(this);
            GC.SuppressFinalize(this);
        }
    }
}