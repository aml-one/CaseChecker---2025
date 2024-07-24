using CaseChecker.MVVM.Core;
using CaseChecker.MVVM.Model;
using CaseChecker.MVVM.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace CaseChecker.MVVM.ViewModel;

public class ManagementViewModel : ObservableObject
{
    private static ManagementViewModel? instance;
    public static ManagementViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private List<DeviceInfoModel> authenticatedDevices = [];
    public List<DeviceInfoModel> AuthenticatedDevices
    {
        get => authenticatedDevices;
        set
        {
            authenticatedDevices = value;
            RaisePropertyChanged(nameof(AuthenticatedDevices));
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
    
    private string latestAppVersionSecondary = string.Empty;
    public string LatestAppVersionSecondary
    {
        get => latestAppVersionSecondary;
        set
        {
            latestAppVersionSecondary = value;
            RaisePropertyChanged(nameof(LatestAppVersionSecondary));
        }
    }
    
    private string newDeviceID = string.Empty;
    public string NewDeviceID
    {
        get => newDeviceID;
        set
        {
            newDeviceID = value;
            RaisePropertyChanged(nameof(NewDeviceID));
        }
    }
    
    private string newFriendlyNameOfDevice = string.Empty;
    public string NewFriendlyNameOfDevice
    {
        get => newFriendlyNameOfDevice;
        set
        {
            newFriendlyNameOfDevice = value;
            RaisePropertyChanged(nameof(NewFriendlyNameOfDevice));
        }
    }
    
    private int selectedAccessLevelIndex;
    public int SelectedAccessLevelIndex
    {
        get => selectedAccessLevelIndex;
        set
        {
            selectedAccessLevelIndex = value;
            RaisePropertyChanged(nameof(SelectedAccessLevelIndex));
        }
    }

    private List<DesignerModel> designersModel = [];
    public List<DesignerModel> DesignersModel
    {
        get => designersModel;
        set
        {
            designersModel = value;
            RaisePropertyChanged(nameof(DesignersModel));
        }
    }
    
    private List<string> designers = [];
    public List<string> Designers
    {
        get => designers;
        set
        {
            designers = value;
            RaisePropertyChanged(nameof(Designers));
        }
    }
    
    private DeviceInfoModel? selectedDevice;
    public DeviceInfoModel SelectedDevice
    {
        get => selectedDevice!;
        set
        {
            selectedDevice = value;
            RaisePropertyChanged(nameof(SelectedDevice));
            if (value != null)
            {
                //SelectedAccessLevelIndex = SelectedDevice.AccessTo switch
                //{
                //    "Left" => 0,
                //    "Right" => 1,
                //    "Both" => 2,
                //    _ => -1,
                //};

                if (DesignersModel.FirstOrDefault(x => x.DesignerID == SelectedDevice.AccessTo) != null)
                {
                    SelectedAccessLevelIndex = DesignersModel.IndexOf(DesignersModel.FirstOrDefault(x => x.DesignerID == SelectedDevice.AccessTo));
                }
                else if (SelectedDevice.AccessTo == "Both")
                {
                    SelectedAccessLevelIndex = DesignersModel.Count;
                }
                else
                {
                    SelectedAccessLevelIndex = -1;
                }
            }
        }
    }

    public RelayCommand RevokeDeviceAccessCommand { get; set; }
    public RelayCommand ChangeAccessLevelCommand { get; set; }
    public RelayCommand ClearSelectionCommand { get; set; }
    public RelayCommand AuthenticateNewDeviceCommand { get; set; }
    public RelayCommand ChangeFriendlyNameCommand { get; set; }

    public System.Timers.Timer _timer;

    public ManagementViewModel()
    {
        Instance = this;
        LookForLatestAppVersionInfo();
        _ = GetRegisteredDevicesList();

        _timer = new System.Timers.Timer(20000);
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();

        RevokeDeviceAccessCommand = new RelayCommand(o => RevokeDeviceAccess());
        ChangeAccessLevelCommand = new RelayCommand(o => ChangeAccessLevel());
        ClearSelectionCommand = new RelayCommand(o => ClearSelection());
        AuthenticateNewDeviceCommand = new RelayCommand(o => AuthenticateNewDevice());
        ChangeFriendlyNameCommand = new RelayCommand(o => ChangeNameOfDevice());
        
        DesignersModel = MainViewModel.Instance.DesignersModel;

        foreach (var designer in DesignersModel)
        {
            Designers.Add(designer.FriendlyName);
        }

        Designers.Add("Admin");
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (SelectedDevice == null)
            _ = GetRegisteredDevicesList();
        LookForLatestAppVersionInfo();
    }

    private async void AuthenticateNewDevice()
    {
        if (NewDeviceID.Length < 32)
        {
            MessageBox.Show(ManagementWindow.Instance, "This device id is invalid!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        await Task.Run(async () =>
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            var http = new HttpClient(handler);
            http.DefaultRequestHeaders.Add("DeviceId", MainViewModel.Instance.DeviceId);

            try
            {

                List<DeviceInfoModel> devices = new();
                try
                {
                    string result = await http.GetStringAsync($"https://{MainViewModel.Instance.ServerAddress}:10113/api/registereddeviceslist/register/{NewDeviceID}");

                    if (result == "true")
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show(ManagementWindow.Instance, "Device id successfully enrolled!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }));
                        await GetRegisteredDevicesList();
                        return;
                    }

                    if (result == "exists")
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show(ManagementWindow.Instance, "This device id is already registered!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                        return;
                    }

                    if (result == "false")
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show(ManagementWindow.Instance, "Something went wrong! Try again later..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                        return;
                    }


                }
                catch (Exception ex) 
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#101e: " + ex.Message);
                        MessageBox.Show(ManagementWindow.Instance, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MainViewModel.Instance.AddToDebug("#102e: " + ex.Message);
                }));
            }

            http.Dispose();
            handler.Dispose();
        });
    }

    private void ClearSelection()
    {
        SelectedDevice = null;
    }

    private async void ChangeNameOfDevice()
    {
        if (string.IsNullOrEmpty(NewFriendlyNameOfDevice.Trim()))
        {
            MessageBox.Show(ManagementWindow.Instance, $"Invalid name.. Please change it, and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string deviceName = SelectedDevice.FriendlyName!;
        if (string.IsNullOrEmpty(deviceName))
        {
            deviceName = SelectedDevice.Name! + " - " + SelectedDevice.DeviceId!;
        }


        MessageBoxResult result = MessageBox.Show(ManagementWindow.Instance, $"Are you sure you want to change the name to:\n\n{deviceName}?", "Change name", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            await Task.Run(async () =>
            {
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };

                var http = new HttpClient(handler);
                http.DefaultRequestHeaders.Add("DeviceId", MainViewModel.Instance.DeviceId);

                try
                {
                    string result = "";
                    List<DeviceInfoModel> devices = new();
                    try
                    {
                        result = await http.GetStringAsync($"https://{MainViewModel.Instance.ServerAddress}:10113/api/registereddeviceslist/namechange/{SelectedDevice.DeviceId}/{NewFriendlyNameOfDevice.Trim()}");
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MainViewModel.Instance.AddToDebug("#1015e: " + ex.Message);
                        }));
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result == "true")
                        {
                            MessageBox.Show(ManagementWindow.Instance, $"Friendly name successfully changed to:\n\n{deviceName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show(ManagementWindow.Instance, $"An error occured during the process..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        SelectedDevice = null;
                    }));
                    await GetRegisteredDevicesList();
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#1016e: " + ex.Message);
                        MessageBox.Show(ManagementWindow.Instance, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }

                http.Dispose();
                handler.Dispose();
            });
            NewFriendlyNameOfDevice = "";
        }
    }
    
    private async void ChangeAccessLevel()
    {
        int count = DesignersModel.Count -1;
        string accessLevel = "";
        if (SelectedAccessLevelIndex > count && selectedAccessLevelIndex != -1)
        {
            accessLevel = "Both";
        }
        else if (SelectedAccessLevelIndex <= count && SelectedAccessLevelIndex != -1)
        {
            accessLevel = DesignersModel[SelectedAccessLevelIndex].DesignerID;
        }

        string deviceName = SelectedDevice.FriendlyName!;
        if (string.IsNullOrEmpty(deviceName))
        {
            deviceName = SelectedDevice.Name! + " - " + SelectedDevice.DeviceId!;
        }

        MessageBoxResult result = MessageBox.Show(ManagementWindow.Instance, $"Are you sure you want to change access to:\n\n{deviceName}?", "Change access", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            await Task.Run(async () =>
            {
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };

                var http = new HttpClient(handler);
                http.DefaultRequestHeaders.Add("DeviceId", MainViewModel.Instance.DeviceId);

                try
                {
                    string result = "";
                    List<DeviceInfoModel> devices = new();
                    try
                    {
                        result = await http.GetStringAsync($"https://{MainViewModel.Instance.ServerAddress}:10113/api/registereddeviceslist/accesslevel/{SelectedDevice.DeviceId}/{accessLevel}");
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MainViewModel.Instance.AddToDebug("#1013e: " + ex.Message);
                        }));
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result == "true")
                        {
                            MessageBox.Show(ManagementWindow.Instance, $"Access successfully changed to:\n\n{deviceName}", "Access Level Changed", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show(ManagementWindow.Instance, $"An error occured during the process..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        SelectedDevice = null;
                    }));
                    await GetRegisteredDevicesList();
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#1014e: " + ex.Message);
                        MessageBox.Show(ManagementWindow.Instance, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }

                http.Dispose();
                handler.Dispose();
            });
        }
    }

    private async void RevokeDeviceAccess()
    {
        if (SelectedDevice.DeviceId == MainViewModel.Instance.DeviceId)
        {
            MessageBox.Show(ManagementWindow.Instance, "You cannot revoke your device's access!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string deviceName = SelectedDevice.FriendlyName!;
        if (string.IsNullOrEmpty(deviceName))
        {
            deviceName = SelectedDevice.Name! + " - " + SelectedDevice.DeviceId!;
        }


        MessageBoxResult result = MessageBox.Show(ManagementWindow.Instance, $"Are you sure you want to revoke access to:\n\n{deviceName}?", "Revoke access", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            await Task.Run(async () =>
            {
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };

                var http = new HttpClient(handler);
                http.DefaultRequestHeaders.Add("DeviceId", MainViewModel.Instance.DeviceId);

                try
                {

                    List<DeviceInfoModel> devices = new();
                    try
                    {
                        string result = await http.GetStringAsync($"https://{MainViewModel.Instance.ServerAddress}:10113/api/registereddeviceslist/revoke/{SelectedDevice.DeviceId}");
                    }
                    catch (Exception ex) 
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MainViewModel.Instance.AddToDebug("#103e: " + ex.Message);
                        }));
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show(ManagementWindow.Instance, $"Access successfully revoked from:\n\n{deviceName}", "Access revoked", MessageBoxButton.OK, MessageBoxImage.Information);
                        SelectedDevice = null;
                    }));
                    await GetRegisteredDevicesList();
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#104e: " + ex.Message);
                        MessageBox.Show(ManagementWindow.Instance, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }

                http.Dispose();
                handler.Dispose();
            });
        }
    }

    private async void LookForLatestAppVersionInfo()
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
                LatestAppVersion = remVersion;
            }));
        }
        catch (Exception)
        {
            LatestAppVersion = "-";
            try
            {
                string result = await new HttpClient().GetStringAsync("https://aml.one/CaseChecker/version.txt");
                _ = double.TryParse(result[..result.IndexOf('-')].Trim(), out remoteVersion);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    string remVersion = remoteVersion.ToString();
                    if (!remVersion.Contains('.'))
                        remVersion += ".0";
                    LatestAppVersionSecondary = remVersion;
                }));
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MainViewModel.Instance.AddToDebug("#100e: " + ex.Message);
                }));
            }
        }

        try
        {
            string result = await new HttpClient().GetStringAsync("https://aml.one/CaseChecker/version.txt");
            _ = double.TryParse(result[..result.IndexOf('-')].Trim(), out remoteVersion);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                string remVersion = remoteVersion.ToString();
                if (!remVersion.Contains('.'))
                    remVersion += ".0";
                LatestAppVersionSecondary = remVersion;
            }));
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainViewModel.Instance.AddToDebug("#100e: " + ex.Message);
            }));
        }
    }

    public async Task GetRegisteredDevicesList()
    {
        await Task.Run(async () =>
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            var http = new HttpClient(handler);
            http.DefaultRequestHeaders.Add("DeviceId", MainViewModel.Instance.DeviceId);

            try
            {

                List<DeviceInfoModel> devices = [];
                try
                {
                    string result = await http.GetStringAsync($"https://{MainViewModel.Instance.ServerAddress}:10113/api/registereddeviceslist/get/0");

                    devices = JsonConvert.DeserializeObject<List<DeviceInfoModel>>(result)!;

                    foreach (var device in devices)
                    {
                        if (device.DeviceId == MainViewModel.Instance.DeviceId)
                            device.IsItTheHostDevice = "True";
                    }
                }
                catch (Exception ex) 
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainViewModel.Instance.AddToDebug("#50e: " + ex.Message);
                    }));
                }

                AuthenticatedDevices = devices;
            }
            catch (Exception exx)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MainViewModel.Instance.AddToDebug("#51e: " + exx.Message);
                }));
            }

            http.Dispose();
            handler.Dispose();
        });
    }
}
