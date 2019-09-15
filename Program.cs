// <copyright file="Program.cs" company="N/A">
// Copyright (c) Simon Wendel. All rights reserved.
// </copyright>

namespace ComboInterceptor
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var path = ConfigurationManager.AppSettings.Get("executablePath");
            var asSingleton = bool.Parse(ConfigurationManager.AppSettings.Get("runAsSingleton"));
            var graceTime = int.Parse(ConfigurationManager.AppSettings.Get("processGraceTimeMillis"));

            var runner = new Runner(path, asSingleton, graceTime);

            var interceptor = new Interceptor();
            interceptor.KeyComboIntercepted += (s, e) => runner.Run();

            var keyComboHelpText = "Win + E";
            var systemTray = new SystemTray(keyComboHelpText);
            systemTray.ExitRequested += (s, e) =>
            {
                interceptor.Stop();
                Application.Exit();
            };

            interceptor.Start((int)Keys.LWin, (int)Keys.E);
            Application.Run(systemTray);
        }
    }
}
