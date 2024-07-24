using CaseChecker.MVVM.Core;
using CaseChecker.MVVM.View;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace CaseChecker.MVVM.ViewModel;

public class LoginViewModel : ObservableObject
{
    public static readonly string LocalConfigFolderHelper = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Stats_CaseChecker\";

    private static LoginViewModel? instance;
    public static LoginViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

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

    private string deviceId = "";
    public string DeviceId
    {
        get => deviceId;
        set
        {
            deviceId = value;
            RaisePropertyChanged(nameof(DeviceId));
        }
    }
    
    private string deviceInfo = "";
    public string DeviceInfo
    {
        get => deviceInfo;
        set
        {
            deviceInfo = value;
            RaisePropertyChanged(nameof(DeviceInfo));
        }
    }
    private bool isNotWorking = true;
    public bool IsNotWorking
    {
        get => isNotWorking;
        set
        {
            isNotWorking = value;
            RaisePropertyChanged(nameof(IsNotWorking));
        }
    }
    
    private string messages = "";
    public string Messages
    {
        get => messages;
        set
        {
            messages = value;
            RaisePropertyChanged(nameof(Messages));
        }
    }
    
    private string messageColor = "Beige";
    public string MessagesColor
    {
        get => messageColor;
        set
        {
            messageColor = value;
            RaisePropertyChanged(nameof(MessagesColor));
        }
    }
    
    private string serverAddress = "";
    public string ServerAddress
    {
        get => serverAddress;
        set
        {
            serverAddress = value;
            RaisePropertyChanged(nameof(ServerAddress));
        }
    }
    
    private string accessLevel = "";
    public string AccessLevel
    {
        get => accessLevel;
        set
        {
            accessLevel = value;
            RaisePropertyChanged(nameof(AccessLevel));
        }
    }

    public RelayCommand ConnectToServerCommand { get; set; }

    public LoginViewModel()
    {
        Instance = this;
        Directory.CreateDirectory(LocalConfigFolderHelper);

        DeviceId = GetDeviceId.GetDeviceID();
        DeviceInfo = GetDeviceId.ReadDeviceInfo();

        ConnectToServerCommand = new RelayCommand(o => ConnectToServer());

        if (File.Exists($"{LocalConfigFolderHelper}serverAddress.cf"))
            ServerAddress = File.ReadAllText($"{LocalConfigFolderHelper}serverAddress.cf");

        Lang = LoginWindow.Lang;
    }

    private void ConnectToServer()
    {
        Login();
    }

    public void AutoLogin()
    {
        if (Directory.Exists(LocalConfigFolderHelper))
            File.WriteAllText($"{LocalConfigFolderHelper}autoLogin.cf", "1");

        Login();
    }

    public async void Login()
    {
        File.WriteAllText($"{LocalConfigFolderHelper}serverAddress.cf", ServerAddress);

        IsNotWorking = false;

        try
        {

            if (ServerAddress.StartsWith("http://"))
                ServerAddress = ServerAddress.Replace("http://", "");

            if (ServerAddress.StartsWith("https://"))
                ServerAddress = ServerAddress.Replace("https://", "");

            ServerAddress = ServerAddress.Replace("/", "").Trim();
        }
        catch (Exception)
        {
        }

        

        Messages = (string)Lang["checkingServer"];
        MessagesColor = "LightGreen";
        if (await CheckIfServerIsAlive())
        {
            Messages = (string)Lang["checkingIfDeviceRegistered"];
            MessagesColor = "LightBlue";

            if (await CheckIfDeviceIsRegistered())
            {
                Messages = (string)Lang["attemtingToLogin"];
                MessagesColor = "LightGreen";

                if (await GetBackDeviceAccessLevel())
                {
                    Messages = (string)Lang["loginSuccess"];
                    MessagesColor = "LightPurple";
                    
                    LoginWindow.Instance.Hide();
                    MainWindow mainWindow = new();
                    mainWindow.Show();
                                        
                    IsNotWorking = true;
                }
                else
                {
                    Messages = (string)Lang["errorOccuredDuringLogin"];
                    MessagesColor = "IndianRed";
                    IsNotWorking = true;
                }
            }
            else
            {
                Messages = (string)Lang["deviceNotRegistered"];
                MessagesColor = "IndianRed";
                IsNotWorking = true;
            }
        }
        else
        {
            Messages = (string)Lang["couldNotConnectToServer"];
            MessagesColor = "IndianRed";
            IsNotWorking = true;
        }
    }

    private async Task<bool> GetBackDeviceAccessLevel()
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var http = new HttpClient(handler);
        http.DefaultRequestHeaders.Add("DeviceId", DeviceId);
        
        try
        {
            string result = await http.GetStringAsync($"https://{ServerAddress}:10113/api/getdeviceaccesslevel/{DeviceId}");

            if (!string.IsNullOrEmpty(result))
            {
                AccessLevel = result;
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MVM {ex.Message}");
        }
        http.Dispose();
        handler.Dispose();
        return false;
    }

    private async Task<bool> CheckIfServerIsAlive()
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var http = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        try
        {
            string result = await http.GetStringAsync($"https://{ServerAddress}:10113/api/checkifserveralive");

            if (result == "true")
                return true;
            else
                return false;

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MVM {ex.Message}");
        }
        http.Dispose();
        handler.Dispose();
        return false;
    }

    public async Task<bool> CheckIfDeviceIsRegistered()
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var http = new HttpClient(handler);
        http.DefaultRequestHeaders.Add("DeviceId", DeviceId);
        string DeviceInfoHash = "";

        try
        {
            
            var plainTextBytes = Encoding.UTF8.GetBytes(DeviceInfo);
            DeviceInfoHash = Convert.ToBase64String(plainTextBytes).Replace("=", "_");
            
            string result = await http.GetStringAsync($"https://{ServerAddress}:10113/api/checkdeviceauthstatus/{DeviceId}/{DeviceInfoHash}");

            if (result == "true")
                return true;
            else
                return false;

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MVM {ex.Message}");
        }
        http.Dispose();
        handler.Dispose();
        return false;
    }
}
