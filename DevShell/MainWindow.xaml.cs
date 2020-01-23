using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace DevShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged, IDisposable
    {
        private readonly IShell _shell;
        private string _time;

        public MainWindow()
        {
            _shell = new Shell();
            InitializeComponent();

        }

        private void OnProcessStarted(SysApp obj)
        {
            RunningApps = _shell.RunningProcesses.ToList();
        }


        private void SysTimer_Tick(object state)
        {
            Dispatcher?.Invoke(() =>
            {
                Time = DateTime.Now.ToShortTimeString();
            });
        }

        public IList<SysApp> RunningApps
        {
            get => _runningApps;
            set
            {
                _runningApps = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllApps));
            }
        }
        public IList<SysApp> DockedApps
        {
            get
            {
                return dockedApps;
            }
            set
            {
                dockedApps = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AllApps));
            }
        }
        public IList<SysApp> AllApps
        {
            get
            {
                return DockedApps.Union(RunningApps).ToList();
            }
           
        }



        private Timer _sysClock;

        public string Time
        {
            get => _time;
            set
            {
                _time = value;

                OnPropertyChanged();
            }
        }

        private void ButtonOpenCmdPrompt_OnClick(object sender, RoutedEventArgs e)
        {
            _shell.Run("cmd.exe");

        }

        private void ButtonOpenInternet_OnClick(object sender, RoutedEventArgs e)
        {
            _shell.Run("https://google.com");
        }

        private void ButtonOpenExplorer_OnClick(object sender, RoutedEventArgs e)
        {
            _shell.Run("c:\\Windows\\explorer.exe");
        }

        private void ButtonOpenTaskManager_OnClick(object sender, RoutedEventArgs e)
        {
            _shell.Run($"{Environment.SystemDirectory}\\taskmgr.exe", "runas", "/7");
        }

        private void ButtonOpenSoundCloud_OnClick(object sender, RoutedEventArgs e)
        {

            //var psi = new ProcessStartInfo(
            //    $"\"C:\\Program Files (x86)\\Microsoft\\Edge Beta\\Application\\msedge_proxy.exe\"",
            //    "  --profile-directory=Default --app-id=gelefkebgmookoikbpknmcdjpfljemjd");
            //psi.WorkingDirectory = @"C:\Program Files (x86)\Microsoft\Edge Beta\Application";
            //psi.UseShellExecute = false;
            //_shell.Run(
            //    psi);
        }
        private AppDrawer _appDrawer;
        private IList<SysApp> _runningApps = new List<SysApp>();
        private IList<SysApp> dockedApps = new List<SysApp>();
        

        private void ButtonOpenAppDrawer_OnClick(object sender, RoutedEventArgs e)
        {
            if (_appDrawer != null)
            {
                if (_appDrawer.IsVisible)
                {
                    _appDrawer.Close();
                }

                else
                {
                    _appDrawer = new AppDrawer();
                    _appDrawer.Top = Height - _appDrawer.Height - 60;
                    _appDrawer.Show();
                }
            }
            else
            {
                _appDrawer = new AppDrawer();
                _appDrawer.Top = Height - _appDrawer.Height - 60;
                _appDrawer.Show();
            }


        }

        private void ButtonOpenPowerShell_OnClick(object sender, RoutedEventArgs e)
        {
            _shell.Run("powershell.exe");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            foreach (var p in _shell.RunningProcesses)
            {
                p.Process?.Kill(true);
            }
        }

        private void ConsoleControlEmbedded_OnProcessInput(object sender, ConsoleControlAPI.ProcessEventArgs args)
        {

            if (args.Content == "clear")
            {
                ConsoleControlEmbedded.ClearOutput();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Time = DateTime.Now.ToShortTimeString();
            _sysClock = new Timer(SysTimer_Tick, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
            _shell.OnProcessStarted = OnProcessStarted;



            ConsoleControlEmbedded.StartProcess(new ProcessStartInfo("powershell.exe"));

            var dockAppFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), @"Resources\Dock\Programs"));

            var dockApps = dockAppFiles.Where(f => !f.EndsWith("desktop.ini")). Select(f =>
            {
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(f);

                return new SysApp()
                {
                    DisplayName = Path.GetFileNameWithoutExtension(f)
                       ,
                    Icon = icon,
                    ImageSource = icon.ToImageSource(),
                    
                };
            });
            DockedApps = dockApps.ToList();
        }
        private int _cInit = 0;
        private void ConsoleControlEmbedded_OnProcessOutput(object sender, ConsoleControlAPI.ProcessEventArgs args)
        {
            if (_cInit == 2)
            {
                ConsoleControlEmbedded.ClearOutput();
                ConsoleControlEmbedded.WriteOutput("DevShell>", System.Windows.Media.Color.FromRgb(255, 255, 255));
                _cInit++;
            }
            else if (_cInit < 2)
            {
                _cInit++;
            }
        }

        public void Dispose()
        {
            _sysClock?.Dispose();
            _shell?.Dispose();
        }
    }


    //public interface IShell
    //{
    //    void Run(string exePath, string verb = null, params string[] args);

    //    void Run(ProcessStartInfo psi);

    //    IReadOnlyCollection<SysApp> RunningProcesses { get; }
    //    Action<SysApp> OnProcessStarted { get; set; }
    //}

    //public class Shell : IShell
    //{
    //    private readonly List<SysApp> _rp = new List<SysApp>();

    //    public Shell()
    //    {

    //    }
    //    /// <inheritdoc />
    //    public void Run(string exePath, string verb = null, params string[] args)
    //    {
    //        var psi = new ProcessStartInfo(exePath, string.Join(" ", args)) { Verb = verb };

    //        var p = Process.Start(psi);
    //        p.Exited += OnProcessExit;
    //        var path= FileUtilities.WhereSearch(p.StartInfo.FileName);
    //        var app = new SysApp() { DisplayName = p.MainWindowTitle, Icon = Icon.ExtractAssociatedIcon(path) };
    //        app.ImageSource = app.Icon.ToImageSource();
    //        _rp.Add(app);

    //        OnProcessStarted?.Invoke(app);
    //    }

    //    private void OnProcessExit(object sender, EventArgs e)
    //    {
    //        var x = 1;
    //    }

    //    /// <inheritdoc />
    //    public void Run(ProcessStartInfo psi)
    //    {
    //        var p = Process.Start(psi);
    //        p.Exited += OnProcessExit;
    //        var app = new SysApp() { DisplayName = p.MainWindowTitle, Icon = Icon.FromHandle(p.MainWindowHandle) };
    //        app.ImageSource = app.Icon.ToImageSource();
    //        _rp.Add(app);
    //        OnProcessStarted?.Invoke(app);
    //    }

    //    public Action<SysApp> OnProcessStarted { get; set; }



    //    /// <inheritdoc />
    //    public IReadOnlyCollection<SysApp> RunningProcesses => _rp.AsReadOnly();
    //}

    //public static class FileUtilities
    //{
    //    public static string WhereSearch(string filename)
    //    {
    //        var paths = new[] { Environment.CurrentDirectory }
    //            .Concat(Environment.GetEnvironmentVariable("PATH").Split(';'));
    //        var extensions = new[] { String.Empty }
    //            .Concat(Environment.GetEnvironmentVariable("PATHEXT").Split(';')
    //                .Where(e => e.StartsWith(".")));
    //        var combinations = paths.SelectMany(x => extensions,
    //            (path, extension) => System.IO.Path.Combine(path, filename + extension));
    //        return combinations.FirstOrDefault(File.Exists);
    //    }
    //}

    //public static class IconHelper
    //{
    //    public static ImageSource GetIconImageSource(Process p)
    //    {
    //        return Icon.FromHandle(p.MainWindowHandle).ToImageSource();
    //    }
    //}
}
