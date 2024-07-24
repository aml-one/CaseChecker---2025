using CaseChecker.MVVM.Core;
using CaseChecker.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CaseChecker.MVVM.ViewModel;

public class PassCodeViewModel : ObservableObject
{
    private string? passCode;
    public string PassCode
    {
        get => passCode;
        set
        {
            passCode = value;
            RaisePropertyChanged(nameof(PassCode));
            if (PassCode.Length >= 8)
                CheckPassCode();
        }
    }

    private string? digit1Saved;
    public string Digit1Saved
    {
        get => digit1Saved;
        set
        {
            if (value != "•")
                digit1Saved = value;
            RaisePropertyChanged(nameof(Digit1Saved));
            BuildPassCode();
        }
    }

    private string? digit1;
    public string Digit1
    {
        get => digit1;
        set
        {
            digit1 = value;
            if (value != "")
            {
                Digit1Saved = value;
                digit1 = "•";
            }
            RaisePropertyChanged(nameof(Digit1));
        }
    }

    private string? digit2Saved;
    public string Digit2Saved
    {
        get => digit2Saved;
        set
        {
            if (value != "•")
                digit2Saved = value;
            RaisePropertyChanged(nameof(Digit2Saved));
            BuildPassCode();
        }
    }

    private string? digit2;
    public string Digit2
    {
        get => digit2;
        set
        {
            digit2 = value;
            if (value != "")
            {
                Digit2Saved = value;
                digit2 = "•";
            }
            RaisePropertyChanged(nameof(Digit2));
        }
    }

    private string? digit3Saved;
    public string Digit3Saved
    {
        get => digit3Saved;
        set
        {
            if (value != "•")
                digit3Saved = value;
            RaisePropertyChanged(nameof(Digit3Saved));
            BuildPassCode();
        }
    }

    private string? digit3;
    public string Digit3
    {
        get => digit3;
        set
        {
            digit3 = value;
            if (value != "")
            {
                Digit3Saved = value;
                digit3 = "•";
            }
            RaisePropertyChanged(nameof(Digit3));
        }
    }

    private string? digit4Saved;
    public string Digit4Saved
    {
        get => digit4Saved;
        set
        {
            if (value != "•")
                digit4Saved = value;
            RaisePropertyChanged(nameof(Digit4Saved));
            BuildPassCode();
        }
    }

    private string? digit4;
    public string Digit4
    {
        get => digit4;
        set
        {
            digit4 = value;
            if (value != "")
            {
                Digit4Saved = value;
                digit4 = "•";
            }
            RaisePropertyChanged(nameof(Digit4));
        }
    }

    private string? digit5Saved;
    public string Digit5Saved
    {
        get => digit5Saved;
        set
        {
            if (value != "•")
                digit5Saved = value;
            RaisePropertyChanged(nameof(Digit5Saved));
            BuildPassCode();
        }
    }

    private string? digit5;
    public string Digit5
    {
        get => digit5;
        set
        {
            digit5 = value;
            if (value != "")
            {
                Digit5Saved = value;
                digit5 = "•";
            }
            RaisePropertyChanged(nameof(Digit5));
        }
    }

    private string? digit6Saved;
    public string Digit6Saved
    {
        get => digit6Saved;
        set
        {
            if (value != "•")
                digit6Saved = value;
            RaisePropertyChanged(nameof(Digit6Saved));
            BuildPassCode();
        }
    }

    private string? digit6;
    public string Digit6
    {
        get => digit6;
        set
        {
            digit6 = value;
            if (value != "")
            {
                Digit6Saved = value;
                digit6 = "•";
            }
            RaisePropertyChanged(nameof(Digit6));
        }
    }

    private string? digit7Saved;
    public string Digit7Saved
    {
        get => digit7Saved;
        set
        {
            if (value != "•")
                digit7Saved = value;
            RaisePropertyChanged(nameof(Digit7Saved));
            BuildPassCode();
        }
    }

    private string? digit7;
    public string Digit7
    {
        get => digit7;
        set
        {
            digit7 = value;

            if (value != "")
            {
                Digit7Saved = value;
                digit7 = "•";
            }
            RaisePropertyChanged(nameof(Digit7));
        }
    }

    private string? digit8Saved;
    public string Digit8Saved
    {
        get => digit8Saved;
        set
        {
            if (value != "•")
                digit8Saved = value;
            RaisePropertyChanged(nameof(Digit8Saved));
            BuildPassCode();
        }
    }
    
    private string? digit8;
    public string Digit8
    {
        get => digit8;
        set
        {
            digit8 = value;

            if (value != "")
            {
                Digit8Saved = value;
                digit8 = "•";
            }
            RaisePropertyChanged(nameof(Digit8));
        }
    }
    
    private string? goal;
    public string Goal
    {
        get => goal;
        set
        {
            goal = value;
            RaisePropertyChanged(nameof(Goal));
        }
    }

    public PassCodeViewModel()
    {
        Goal = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
    }

    private void CheckPassCode()
    {
        if (PassCode == DateTime.Now.AddDays(1).ToString("yyyyMMdd"))
        {
            ClearDigits();
            ManagementWindow managementWindow = new()
            {
                Owner = MainWindow.Instance
            };
            PassCodeWindow.Instance.Close();
            managementWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show("Invalid code");
            ClearDigits();
        }
    }

    private void ClearDigits()
    {
        PassCode = "";
        Digit1 = "";
        Digit2 = "";
        Digit3 = "";
        Digit4 = "";
        Digit5 = "";
        Digit6 = "";
        Digit7 = "";
        Digit8 = "";
        Digit1Saved = "";
        Digit2Saved = "";
        Digit3Saved = "";
        Digit4Saved = "";
        Digit5Saved = "";
        Digit6Saved = "";
        Digit7Saved = "";
        Digit8Saved = "";
    }

    private void BuildPassCode()
    {
        PassCode = $"{Digit1Saved}{Digit2Saved}{Digit3Saved}{Digit4Saved}{Digit5Saved}{Digit6Saved}{Digit7Saved}{Digit8Saved}";
    }
}
