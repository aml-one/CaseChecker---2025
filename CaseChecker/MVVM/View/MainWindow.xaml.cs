using CaseChecker.MVVM.Core;
using CaseChecker.MVVM.Model;
using CaseChecker.MVVM.ViewModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static CaseChecker.MVVM.View.MainWindow.NativeMethods;
using System.Windows.Interop;

namespace CaseChecker.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly string LocalConfigFolderHelper = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Stats_CaseChecker\";

        private ResourceDictionary lang = [];
        public ResourceDictionary Lang
        {
            get => lang;
            set
            {
                lang = value;
                RaisePropertyChanged(nameof(Lang));
            }
        }

        private double remoteAppVersionDouble;
        public double RemoteAppVersionDouble
        {
            get => remoteAppVersionDouble;
            set
            {
                remoteAppVersionDouble = value;
                RaisePropertyChanged(nameof(RemoteAppVersionDouble));
            }
        }

        private string latestAppVersion = string.Empty;
        public string LatestAppVersion
        {
            get => latestAppVersion;
            set
            {
                latestAppVersion = value;
                RaisePropertyChanged(nameof(LatestAppVersion));
            }
        }

        private Dictionary<string, bool> expandStatesLeft = [];
        private Dictionary<string, bool> expandStatesRight = [];
        public System.Timers.Timer _timer;
        public System.Timers.Timer _updateTimer;
        private static bool UpdateMessagePresented = false;
        private static bool AutoUpdateAtStart = false;
        private static bool AppJustStarted = true;

        public static event PropertyChangedEventHandler? PropertyChangedStatic;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
        {
            PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }
        public void RaisePropertyChanged([CallerMemberName] string? propertyname = null)
        {
            PropertyChanged?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        private static MainWindow? instance;
        public static MainWindow Instance
        {
            get => instance!;
            set
            {
                instance = value;
                RaisePropertyChangedStatic(nameof(Instance));
            }
        }
        
        
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            _timer = new System.Timers.Timer(4000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            _updateTimer = new System.Timers.Timer(10000);
            _updateTimer.Elapsed += UpdateTimer_Elapsed;
            _updateTimer.Start();

            PropertyGroupDescription groupDescription = new("SentOn");
            //listViewLeft.Items.GroupDescriptions.Add(groupDescription);
            //listViewRight.Items.GroupDescriptions.Add(groupDescription);


            if (File.Exists($"{LocalConfigFolderHelper}autoLogin.cf") && File.Exists($"{LocalConfigFolderHelper}windowPosition.cf"))
            {
                string[] wndwData = File.ReadAllText($"{LocalConfigFolderHelper}windowPosition.cf").Split('|');
                _ = int.TryParse(wndwData[0], out int wWidth);
                _ = int.TryParse(wndwData[1], out int wHeight);
                _ = int.TryParse(wndwData[2], out int wTop);
                _ = int.TryParse(wndwData[3], out int wLeft);

                if (wndwData[4] != "Maximized")
                {
                    Width = wWidth;
                    Height = wHeight;
                    Top = wTop;
                    Left = wLeft;
                }

                if (wndwData[4] == "Normal")
                    WindowState = WindowState.Normal;
                if (wndwData[4] == "Maximized")
                    WindowState = WindowState.Maximized;

                File.Delete($"{LocalConfigFolderHelper}autoLogin.cf");
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
             
                if (LoginViewModel.Instance.AccessLevel.Equals("both", StringComparison.CurrentCultureIgnoreCase))
                    this.Width = 1200;
                else
                    this.Width = 500;
            }



            if (LoginWindow.Instance.DontDoAutoUpdate)
                AutoUpdateAtStart = true;

            SetLanguageDictionary();
        }

        private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _updateTimer.Interval = (3600 * 1000);
            LookForUpdate();
        }

        private async void LookForUpdate()
        {
            double remoteVersion = 0;
            try
            {
                string result = await new HttpClient().GetStringAsync("https://raw.githubusercontent.com/aml-one/CaseChecker/master/CaseChecker/version.txt");
                _ = double.TryParse(result[..result.IndexOf('-')].Trim(), out remoteVersion);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    string remVersion = remoteVersion.ToString();
                    if (!remVersion.Contains('.'))
                        remVersion += ".0";
                    versionLabel.ToolTip = $"{(string)Lang["lastAvailableVersion"]}: v{remVersion}";
                    LatestAppVersion = remVersion;
                }));
            }
            catch (Exception)
            {
                try
                {
                    string result = await new HttpClient().GetStringAsync("https://aml.one/CaseChecker/version.txt");
                    _ = double.TryParse(result[..result.IndexOf('-')].Trim(), out remoteVersion);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        string remVersion = remoteVersion.ToString();
                        if (!remVersion.Contains('.'))
                            remVersion += ".0";
                        versionLabel.ToolTip = $"{(string)Lang["lastAvailableVersion"]}: v{remVersion}";
                    }));
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#10e: " + ex.Message);
                    }));
                }
            }

            if (remoteVersion > MainViewModel.Instance.AppVersionDouble)
            {
                MainViewModel.Instance.UpdateAvailable = true;
                if (!AppJustStarted)
                {
                    if (!UpdateMessagePresented)
                    {
                        UpdateMessagePresented = true;
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBoxResult result = MessageBox.Show(this, (string)Lang["updateAvailableMessageBox"], (string)Lang["updateAvailableMessageBoxTitle"], MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                MainViewModel.Instance.StartProgramUpdate();
                            }
                        }));
                    }
                }
                
                if (!AutoUpdateAtStart)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#10: Starting auto update");
                        MainViewModel.Instance.StartProgramUpdate();
                    }));
                }

                AppJustStarted = false;
            }
            else
                MainViewModel.Instance.UpdateAvailable = false;

            AutoUpdateAtStart = true;
        }

        public void SetLanguageDictionary(string language = "")
        {
            if (language.Equals(""))
            {
                Lang.Source = Thread.CurrentThread.CurrentCulture.ToString() switch
                {
                    "en-US" => new Uri("..\\..\\Lang\\StringResources_English.xaml", UriKind.Relative),
                    "zh-Hans" => new Uri("..\\..\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-Hant" => new Uri("..\\..\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CHT" => new Uri("..\\..\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CN" => new Uri("..\\..\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CHS" => new Uri("..\\..\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-HK" => new Uri("..\\..\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    _ => new Uri("..\\..\\Lang\\StringResources_English.xaml", UriKind.Relative),
                };
            }
            else
            {
                try
                {
                    Lang.Source = new Uri("..\\..\\Lang\\StringResources_" + language + ".xaml", UriKind.Relative);
                }
                catch (IOException ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#11e: " + ex.Message);
                    }));
                    Lang.Source = new Uri("..\\..\\Lang\\StringResources_English.xaml", UriKind.Relative);
                }
            }

            this.Resources.MergedDictionaries.Add(lang);
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            //try
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => {
            //        //listViewLeft.Items.Refresh();
            //        //listViewRight.Items.Refresh();
            //    }));
            //}
            //catch (Exception ex)
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        MainViewModel.Instance.AddToDebug("#12e: " + ex.Message);
            //    }));
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoginWindow.Instance.Close();
        }

        private void listViewLeft_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // take into account vertical scrollbar

           
            gView.Columns[0].Width = 34;
            gView.Columns[1].Width = workingWidth - 268;
            gView.Columns[2].Width = 45;
            gView.Columns[3].Width = 44;
            gView.Columns[4].Width = 30;
            gView.Columns[5].Width = 110;
        }

        private void listViewRight_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View! as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // take into account vertical scrollbar

            gView!.Columns[0].Width = 34;
            gView.Columns[1].Width = workingWidth - 268;
            gView.Columns[2].Width = 45;
            gView.Columns[3].Width = 44;
            gView.Columns[4].Width = 30;
            gView.Columns[5].Width = 110;
        }

        private void ExpanderLeft_Loaded(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            var dc = (CollectionViewGroup)expander.DataContext;
            var groupName = dc.Name.ToString();
            if (expandStatesLeft.TryGetValue(groupName, out var value))
                expander.IsExpanded = value;
        }

        private void ExpanderLeft_ExpandedCollapsed(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            var dc = (CollectionViewGroup)expander.DataContext;
            var groupName = dc.Name.ToString();
            expandStatesLeft[groupName] = expander.IsExpanded;
        }
        
        private void ExpanderRight_Loaded(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            var dc = (CollectionViewGroup)expander.DataContext;
            var groupName = dc.Name.ToString();
            if (expandStatesRight.TryGetValue(groupName, out var value))
                expander.IsExpanded = value;
        }

        private void ExpanderRight_ExpandedCollapsed(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            var dc = (CollectionViewGroup)expander.DataContext;
            var groupName = dc.Name.ToString();
            expandStatesRight[groupName] = expander.IsExpanded;
        }

        public void TitleBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
                BtnMaximize_Click(sender, e);

            if (e.ChangedButton == MouseButton.Left)
                try
                {
                    this.DragMove();
                }
                catch { }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            double ActualScreenHeight = MaxHeight;
            double ActualScreenWidth = MaxWidth;

            var hwnd = new WindowInteropHelper(this).EnsureHandle();
            var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new NativeMonitorInfo();
                GetMonitorInfo(monitor, monitorInfo);

                //var left = monitorInfo.Monitor.Left;
                //var top = monitorInfo.Monitor.Top;
                ActualScreenWidth = (monitorInfo.Monitor.Right - monitorInfo.Monitor.Left);
                ActualScreenHeight = (monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top);
            }

            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                this.BorderThickness = new Thickness(0);
                btnMaximize.Content = "▣";
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                double LeftMargin = 6;
                double TopMargin = 6;

                if (GetTaskBarLocation() == TaskBarLocation.LEFT)
                    LeftMargin = ActualScreenWidth - MaxWidth + 6;

                if (GetTaskBarLocation() == TaskBarLocation.TOP)
                    TopMargin = ActualScreenHeight - MaxHeight + 6;
                
                    

                this.BorderThickness = new Thickness(LeftMargin, TopMargin, 6, 3);
                btnMaximize.Content = "⧉";

            }

        }

        public static class NativeMethods
        {
            public const Int32 MONITOR_DEFAULTTOPRIMERTY = 0x00000001;
            public const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;


            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);


            [DllImport("user32.dll")]
            public static extern Boolean GetMonitorInfo(IntPtr hMonitor, NativeMonitorInfo lpmi);


            [Serializable, StructLayout(LayoutKind.Sequential)]
            public struct NativeRectangle
            {
                public Int32 Left;
                public Int32 Top;
                public Int32 Right;
                public Int32 Bottom;


                public NativeRectangle(Int32 left, Int32 top, Int32 right, Int32 bottom)
                {
                    this.Left = left;
                    this.Top = top;
                    this.Right = right;
                    this.Bottom = bottom;
                }
            }


            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public sealed class NativeMonitorInfo
            {
                public Int32 Size = Marshal.SizeOf(typeof(NativeMonitorInfo));
                public NativeRectangle Monitor;
                public NativeRectangle Work;
                public Int32 Flags;
            }
        }

        private void BtnCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new Thickness(6, 6, 6, 0);
                btnMaximize.Content = "⧉";
            }
            else if (WindowState == WindowState.Normal)
            {
                this.BorderThickness = new Thickness(0);
                btnMaximize.Content = "▣";
            }

            string windowPosition = $"{Width}|{Height}|{Top}|{Left}|{WindowState}";
            if (Directory.Exists(LocalConfigFolderHelper))
                File.WriteAllText($"{LocalConfigFolderHelper}windowPosition.cf", windowPosition);
        }
        
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            string windowPosition = $"{Width}|{Height}|{Top}|{Left}|{WindowState}";
            if (Directory.Exists(LocalConfigFolderHelper)) 
                File.WriteAllText($"{LocalConfigFolderHelper}windowPosition.cf", windowPosition);
        }

        private void versionLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LookForUpdate();
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MainViewModel.Instance.DebugShows == Visibility.Visible)
                MainViewModel.Instance.DebugShows = Visibility.Collapsed;
            else
                MainViewModel.Instance.DebugShows = Visibility.Visible;
        }

        private enum TaskBarLocation { TOP, BOTTOM, LEFT, RIGHT }

        private TaskBarLocation GetTaskBarLocation()
        {
            //System.Windows.SystemParameters....
            if (SystemParameters.WorkArea.Left > 0)
                return TaskBarLocation.LEFT;
            if (SystemParameters.WorkArea.Top > 0)
                return TaskBarLocation.TOP;
            if (SystemParameters.WorkArea.Left == 0
              && SystemParameters.WorkArea.Width < SystemParameters.PrimaryScreenWidth)
                return TaskBarLocation.RIGHT;
            return TaskBarLocation.BOTTOM;
        }

    }
}
