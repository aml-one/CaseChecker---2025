using CaseChecker.MVVM.Core;
using CaseChecker.MVVM.Model;
using CaseChecker.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaseChecker.MVVM.View
{
    /// <summary>
    /// Interaction logic for UserPanel.xaml
    /// </summary>
    public partial class UserPanel : UserControl, INotifyPropertyChanged
    {
        private Dictionary<string, bool> expandStatesLeft = [];
        public string? DesignerID { get; set; }
        

        public event PropertyChangedEventHandler? PropertyChanged;
        public static event PropertyChangedEventHandler? PropertyChangedStatic;

        public void RaisePropertyChanged([CallerMemberName] string? propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
        {
            PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        private List<CheckedOutCasesModel> sentOutCasesModel = [];
        public List<CheckedOutCasesModel> SentOutCasesModel
        {
            get => sentOutCasesModel;
            set
            {
                sentOutCasesModel = value;
                RaisePropertyChanged(nameof(SentOutCasesModel));
            }
        }

        private static UserPanel instance;
        public static UserPanel Instance
        {
            get => instance;
            set
            {
                instance = value;
                RaisePropertyChangedStatic(nameof(Instance));
            }
        }

        public System.Timers.Timer _timer;

        public UserPanel(string designerID)
        {
            Instance = this;
            SentOutCasesModel = sentOutCasesModel;
            InitializeComponent();
            DesignerID = designerID;

            Debug.WriteLine("UserPanel DesignerID: " + designerID);
            UserPanelViewModel.Instance.DesignerID = designerID;
            SentOutCasesModel = MainViewModel.Instance.SentOutCasesModel;

            MainViewModel.Instance.UserPanels.TryAdd(designerID, this);

            PropertyGroupDescription groupDescription = new("SentOn");
            listView.Items.GroupDescriptions.Add(groupDescription);

            _timer = new System.Timers.Timer(4000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    listView.Items.Refresh();
                    SentOutCasesModel = MainViewModel.Instance.SentOutCasesModel;
                }));
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MainViewModel.Instance.AddToDebug("#12e: " + ex.Message);
                }));
            }
        }

        private void listViewLeft_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView? listView = sender as ListView;
            GridView? gView = listView!.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // take into account vertical scrollbar

            double width = workingWidth - 268;
            if (width < 0) width = 100;


            gView.Columns[0].Width = 34;
            gView.Columns[1].Width = width;
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
            if (expandStatesLeft.TryGetValue(groupName!, out var value))
                expander.IsExpanded = value;
        }

        private void ExpanderLeft_ExpandedCollapsed(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            var dc = (CollectionViewGroup)expander.DataContext;
            var groupName = dc.Name.ToString();
            expandStatesLeft[groupName!] = expander.IsExpanded;
        }
    }
}
