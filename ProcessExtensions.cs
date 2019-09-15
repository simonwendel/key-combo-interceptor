// <copyright file="ProcessExtensions.cs" company="N/A">
// Copyright (c) Simon Wendel. All rights reserved.
// </copyright>

namespace ComboInterceptor
{
    using System.Diagnostics;
    using System.Windows.Automation;

    public static class ProcessExtensions
    {
        public static void BringToFront(this Process process)
        {
            var handle = process.MainWindowHandle;
            var element = AutomationElement.FromHandle(handle);
            element.SetFocus();
        }
    }
}
