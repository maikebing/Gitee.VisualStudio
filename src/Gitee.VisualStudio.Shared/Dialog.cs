﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gitee.VisualStudio.Shared
{
    public class Dialog : ContentControl, IDialog
    {
        public event Action Closed;
        
        public void Close()
        {
            Closed?.Invoke();
        }

        public void Confirm(string message, Action<bool?> callback)
        {
            MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Information(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Warning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
