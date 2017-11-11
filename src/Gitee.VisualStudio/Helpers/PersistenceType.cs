﻿using System.Diagnostics.CodeAnalysis;

namespace Gitee.VisualStudio.Helpers
{
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32"
        , Justification = "This is this way for interop")]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"
        , Justification = "I assume this is defined this way for Native interop")]
    public enum PersistenceType : uint
    {
        Session = 1,
        LocalComputer = 2,
        Enterprise = 3
    }
}