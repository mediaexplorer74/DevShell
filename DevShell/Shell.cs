using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace DevShell
{
    public class Shell : IShell, IDisposable
    {
        private readonly List<SysApp> _rp = new List<SysApp>();

        public Shell()
        {

        }
        /// <inheritdoc />
        public void Run(string exePath, string verb = null, params string[] args)
        {
            var psi = new ProcessStartInfo(exePath, string.Join(" ", args)) { Verb = verb };

            var p = Process.Start(psi);
            p.EnableRaisingEvents = true;
           
            p.Exited += OnProcessExit;
            var path= FileUtilities.WhereSearch(p.StartInfo.FileName);
            var app = new SysApp() { DisplayName = p.MainWindowTitle, Icon = Icon.ExtractAssociatedIcon(path), Process = p};
            app.ImageSource = app.Icon.ToImageSource();
            _rp.Add(app);

            OnProcessStarted?.Invoke(app);

        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            var x = 1;
        }

        /// <inheritdoc />
        public void Run(ProcessStartInfo psi)
        {
            
            var p = Process.Start(psi);
            p.EnableRaisingEvents = true;
            p.Exited += OnProcessExit;
            var icon = (p.MainWindowHandle.ToInt32() != 0) ? Icon.FromHandle(p.MainWindowHandle) : null;

            var app = new SysApp() { DisplayName = p.MainWindowTitle, Icon = icon, Process = p };
            if(app.Icon != null)
            {
                app.ImageSource = app.Icon.ToImageSource();
            }
          
            _rp.Add(app);
            OnProcessStarted?.Invoke(app);
        }

        public Action<SysApp> OnProcessStarted { get; set; }



        /// <inheritdoc />
        public IReadOnlyCollection<SysApp> RunningProcesses => _rp.AsReadOnly();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   
                }

                _rp?.ForEach(p => p?.Process?.Dispose());
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                _rp.Clear();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Shell()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}