// <copyright file="Runner.cs" company="N/A">
// Copyright (c) Simon Wendel. All rights reserved.
// </copyright>

namespace ComboInterceptor
{
    using System.Diagnostics;
    using System.Threading;

    public class Runner
    {
        private readonly string executable;
        private readonly bool asSingleton;
        private readonly int graceTime;
        private Process process;

        public Runner(string executable, bool asSingleton = false, int graceTime = 250)
        {
            this.executable = executable;
            this.asSingleton = asSingleton;
            this.graceTime = graceTime;
        }

        public void Run()
        {
            if (!asSingleton || ShouldRestart())
            {
                StartProcess();
            }

            Thread.Sleep(graceTime);
            process.BringToFront();
        }

        private bool ShouldRestart()
        {
            return process == null || process.HasExited;
        }

        private void StartProcess()
        {
            process = Process.Start(executable);
            process.WaitForInputIdle();
        }
    }
}
