using CaseChecker.MVVM.Core;
using CaseChecker.MVVM.ViewModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace CaseChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private static ResourceDictionary lang = [];
        public static ResourceDictionary Lang
        {
            get => lang;
            set
            {
                lang = value;
                RaisePropertyChangedStatic(nameof(Lang));
            }
        }

        private static LoginWindow? instance;
        public static LoginWindow Instance
        {
            get => instance!;
            set
            {
                instance = value;
                RaisePropertyChangedStatic(nameof(Instance));
            }
        }
        
        private bool dontDoAutoUpdate = false;
        public bool DontDoAutoUpdate
        {
            get => dontDoAutoUpdate!;
            set
            {
                dontDoAutoUpdate = value;
                RaisePropertyChangedStatic(nameof(DontDoAutoUpdate));
            }
        }

        public LoginWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();
            Instance = this;

            if (Environment.GetCommandLineArgs().Length > 1)
                LoginViewModel.Instance.AutoLogin();
        }

        public static event PropertyChangedEventHandler? PropertyChangedStatic;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
        {
            PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        private void SetLanguageDictionary(string language = "")
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
                catch (IOException)
                {
                    Lang.Source = new Uri("..\\..\\Lang\\StringResources_English.xaml", UriKind.Relative);
                }
            }

            this.Resources.MergedDictionaries.Add(lang);
        }

        private void Label_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void labelMadeByAmL_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DontDoAutoUpdate = !DontDoAutoUpdate;

            if (DontDoAutoUpdate)
                labelMadeByAmL.Foreground = Brushes.Beige;
            else
                labelMadeByAmL.Foreground = Brushes.Silver;
        }
    }
}