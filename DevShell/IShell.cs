using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevShell
{
    public interface IShell : IDisposable
    {
        void Run(string exePath, string verb = null, params string[] args);

        void Run(ProcessStartInfo psi);

        IReadOnlyCollection<SysApp> RunningProcesses { get; }
        Action<SysApp> OnProcessStarted { get; set; }
    }
}