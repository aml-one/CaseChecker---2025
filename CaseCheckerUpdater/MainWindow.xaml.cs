using IWshRuntimeLibrary;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using File = System.IO.File;
using Path = System.IO.Path;

namespace CaseCheckerUpdater
{
    public partial class MainWindow : Window
    {
        public static readonly string LocalConfigFolderHelper = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Stats_CaseChecker\";
        public System.Timers.Timer _timer;
        public int AppStartTryCount = 0;
        public string appPath = @"C:\CaseChecker\";

        private ResourceDictionary lang = [];
        public ResourceDictionary Lang
        {
            get => lang;
            set
            {
                lang = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            SetLanguageDictionary();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
        }

        public void SetLanguageDictionary(string language = "")
        {
            if (language.Equals(""))
            {
                Lang.Source = Thread.CurrentThread.CurrentCulture.ToString() switch
                {
                    "en-US" => new Uri("\\Lang\\StringResources_English.xaml", UriKind.Relative),
                    "zh-Hans" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-Hant" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CHT" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CN" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CHS" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-HK" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    _ => new Uri("\\Lang\\StringResources_English.xaml", UriKind.Relative),
                };
            }
            else
            {
                try
                {
                    Lang.Source = new Uri("\\Lang\\StringResources_" + language + ".xaml", UriKind.Relative);
                }
                catch (IOException)
                {
                    Lang.Source = new Uri("\\Lang\\StringResources_English.xaml", UriKind.Relative);
                }
            }

            this.Resources.MergedDictionaries.Add(lang);
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            StopMainApp();
        }

        private void StopMainApp()
        {
            var Processes = Process.GetProcesses()
                               .Where(pr => pr.ProcessName == "CaseChecker");
            foreach (var process in Processes)
            {
                process.Kill();
            }
            
            

            Task.Run(DownloadUpdate).Wait();
            
            StartCaseApp();
        }

        private async void DownloadUpdate()
        {
            Thread.Sleep(2000);

            appPath = @"C:\CaseChecker\";

            if (!Directory.Exists(appPath))
            {
                try
                {
                    Directory.CreateDirectory(appPath);
                }
                catch (Exception) 
                {
                    appPath = Environment.SpecialFolder.Desktop.ToString();
                }
            }

            try
            {
                Thread.Sleep(500);
                if (File.Exists($@"{LocalConfigFolderHelper}CaseChecker_old.exe"))
                    File.Delete($@"{LocalConfigFolderHelper}CaseChecker_old.exe");
                Thread.Sleep(500);
                if (File.Exists($@"{appPath}\CaseChecker.exe"))
                    File.Move($@"{appPath}\CaseChecker.exe", $@"{LocalConfigFolderHelper}CaseChecker_old.exe");
                Thread.Sleep(2000);
                using var client = new HttpClient();
                using var s = await client.GetStreamAsync("https://raw.githubusercontent.com/aml-one/CaseChecker/master/CaseChecker/Executable/CaseChecker.exe");
                using var fs = new FileStream($@"{appPath}CaseChecker.exe", FileMode.OpenOrCreate);
                await s.CopyToAsync(fs);

                CreateShortcut(appPath);
            }
            catch (Exception)
            {
                try
                {
                    Thread.Sleep(500);
                    using var client = new HttpClient();
                    using var s = await client.GetStreamAsync("https://aml.one/CaseChecker/CaseChecker.exe");
                    using var fs = new FileStream($@"{appPath}CaseChecker.exe", FileMode.OpenOrCreate);
                    await s.CopyToAsync(fs);
                    
                    CreateShortcut(appPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (File.Exists($@"{LocalConfigFolderHelper}CaseChecker_old.exe"))
                        File.Move($@"{LocalConfigFolderHelper}CaseChecker_old.exe", $@"{appPath}CaseChecker.exe");
                }
            }

            Thread.Sleep(3000);
        }

        private void CreateShortcut(string appFolder)
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new ();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Case Checker.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Case Checker For Stats";
            shortcut.Hotkey = "Ctrl+Shift+C";
            shortcut.TargetPath = @$"{appFolder}CaseChecker.exe";
            shortcut.Save();
        }

        private void StartCaseApp()
        {
            AppStartTryCount++;
            Thread.Sleep(3000);
            try
            {
                var p = new Process();

                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c \"{appPath}CaseChecker.exe\" updated";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                Thread.Sleep(2000);
                CloseThisApp();
            }
            catch (Exception)
            {
                MessageBox.Show((string)Lang["couldNotStart"], (string)Lang["error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!CheckIfAppIsRunning() && AppStartTryCount < 4)
                StartCaseApp();
        }

        private static bool CheckIfAppIsRunning()
        {
            var Processes = Process.GetProcesses()
                               .Where(pr => pr.ProcessName == "CaseChecker");
            foreach (var process in Processes)
            {
                if (process.Id != null)
                    return true;
            }

            return false;
        }


        private static void CloseThisApp()
        {
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show((string)Lang["closeMessage"], (string)Lang["caseCheckerUpdater"], MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }
    }
}