using CaseChecker.MVVM.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CaseChecker.MVVM.View
{

    public partial class PassCodeWindow : Window, INotifyPropertyChanged
    {
        private static PassCodeWindow? instance;

        public static PassCodeWindow Instance
        {
            get => instance!;
            set
            {
                instance = value;
                RaisePropertyChangedStatic(nameof(Instance));
            }
        }

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

        public static Dictionary<int, TextBox> boxes = [];
        public PassCodeWindow()
        {
            InitializeComponent();
            Instance = this;

            box1.Focus();
        }

        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }


        private void box1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                return;
            if (box1.Text.Length > 0)
                box2.Focus();
        }

        private void box2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                box1.Focus();
            if (box2.Text.Length > 0)
                box3.Focus();
        }
        
        private void box3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                box2.Focus();
            if (box3.Text.Length > 0)
                box4.Focus();
        }
        
        private void box4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                box3.Focus();
            if (box4.Text.Length > 0)
                box5.Focus();
        }

        private void box5_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                box4.Focus();
            if (box5.Text.Length > 0)
                box6.Focus();
        }

        private void box6_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                box5.Focus();
            if (box6.Text.Length > 0)
                box7.Focus();
        }

        private void box7_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                box6.Focus();
            if (box7.Text.Length > 0)
                box8.Focus();
        }
        
        private void box8_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                box7.Focus();
            if (box8.Text.Length > 0)
                box1.Focus();
        }
    }
}
