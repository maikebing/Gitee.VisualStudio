﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gitee.VisualStudio.Shared
{
    public interface IViewFactory
    {
        T GetView<T>(ViewTypes type) where T : Control;
    }
}
