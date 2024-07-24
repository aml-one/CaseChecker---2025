using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using CaseChecker.MVVM.Model;
using CaseChecker.MVVM.View;

namespace CaseChecker.Themes;

public partial class LvItem
{
    public LvItem()
    {
        InitializeComponent();
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem item && item.IsSelected)
        {
            if (item.Content is not null)
            {
                CheckedOutCasesModel model = (CheckedOutCasesModel)item.Content;
                if (!string.IsNullOrEmpty(model.CommentIn3Shape))
                    MessageBox.Show(MainWindow.Instance, model.CommentIn3Shape, model.OrderID, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}