// <copyright file="SystemTray.cs" company="N/A">
// Copyright (c) Simon Wendel. All rights reserved.
// </copyright>

namespace ComboInterceptor
{
    using System;
    using System.Windows.Forms;

    public class SystemTray : ApplicationContext
    {
        private readonly NotifyIcon notifyIcon;

        public SystemTray(string helpText)
        {
            var exitMenuItem = new MenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;

            notifyIcon = new NotifyIcon
            {
                Icon = Resource.MainIcon,
                ContextMenu = new ContextMenu(new MenuItem[] { exitMenuItem }),
                Visible = true,
                Text = $"Intercepts {helpText} and starts another app"
            };
        }

        public event EventHandler ExitRequested;

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
