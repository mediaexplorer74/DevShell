using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Humanizer;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace DevShell
{
    /// <summary>
    /// Interaction logic for AppDrawer.xaml
    /// </summary>
    public partial class AppDrawer : INotifyPropertyChanged
    {
        private IList<SysApp> _apps;

        public AppDrawer()
        {
            InitializeComponent();
            Apps = new List<SysApp>();
        }

        public IList<SysApp> Apps
        {
            get => _apps;
            set
            {
                _apps = value;
                OnPropertyChanged();
            }
        }

        private void LoadApps()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        var add = false;
                        var app = new SysApp();
                        if (subkey.GetValue("DisplayName") is string s)
                        {
                            app.DisplayName = s.Truncate(40);

                            add = true;
                        }

                        if (subkey.GetValue("DisplayIcon") is string iconPath)
                        {
                            app.IconPath = iconPath;
                            try
                            {
                                app.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
                                app.ImageSource = app.Icon.ToImageSource();
                            }
                            catch 
                            {

                            }
                        
                        
                        
                        }
                       
                        if (add)
                        {
                            Apps.Add(app);
                        }
                    }


                }

                var temp = Apps;
                Apps = null;
                Apps = temp
                    .OrderBy(app => app.IsIconVisible)
                    .ThenBy(app => app.DisplayName)
                    .ToList();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AppDrawer_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadApps();
        }
    }

    //public class SysApp
    //{
    //    public string DisplayName { get; set; }

    //    public string IconPath { get; set; }

    //    public bool IsIconVisible => Icon != null;

    //    public Icon Icon { get; set; }
    //    public ImageSource ImageSource { get; set; }

    //}

    internal static class IconUtilities
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();
        
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                                                                          IntPtr.Zero,
                                                                          Int32Rect.Empty,
                                                                          BitmapSizeOptions.FromWidthAndHeight(icon.Width,icon.Height));

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }
    }
}
