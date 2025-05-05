using CaseChecker.MVVM.Core;
using CaseChecker.MVVM.Model;
using CaseChecker.MVVM.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CaseChecker.MVVM.ViewModel;

public partial class UserPanelViewModel : ObservableObject
{
    public System.Timers.Timer _orderTimer;
    public System.Timers.Timer _startTimer;
    public System.Timers.Timer _periodicTimer;
    public static readonly string LocalConfigFolderHelper = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Stats_CaseChecker\";


    #region Properties
    private string designerID = "";
    public string DesignerID
    {
        get => designerID;
        set
        {
            designerID = value;
            RaisePropertyChanged(nameof(DesignerID));
        }
    }

    private string designerName = "";
    public string DesignerName
    {
        get => designerName;
        set
        {
            designerName = value;
            RaisePropertyChanged(nameof(DesignerName));
        }
    }

    private static UserPanelViewModel instance;
    public static UserPanelViewModel Instance
    {
        get => instance;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private static MainViewModel? _mainViewModel;
    public static MainViewModel mainViewModel
    {
        get => _mainViewModel!;
        set
        {
            _mainViewModel = value;
            RaisePropertyChangedStatic(nameof(mainViewModel));
        }
    }

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

    private string language = "English";
    public string Language
    {
        get => language;
        set
        {
            language = value;
            RaisePropertyChanged(nameof(Language));
        }
    }

    #region units

    private double totalUnits = 0;
    public double TotalUnits
    {
        get => totalUnits;
        set
        {
            totalUnits = value;
            RaisePropertyChanged(nameof(TotalUnits));
        }
    }

    private double totalUnitsToday = 0;
    public double TotalUnitsToday
    {
        get => totalUnitsToday;
        set
        {
            totalUnitsToday = value;
            RaisePropertyChanged(nameof(TotalUnitsToday));
        }
    }

    private double totalUnitsLeftOver = 0;
    public double TotalUnitsLeftOver
    {
        get => totalUnitsLeftOver;
        set
        {
            totalUnitsLeftOver = value;
            RaisePropertyChanged(nameof(TotalUnitsLeftOver));
        }
    }



    private double totalCrowns = 0;
    public double TotalCrowns
    {
        get => totalCrowns;
        set
        {
            totalCrowns = value;
            RaisePropertyChanged(nameof(TotalCrowns));
        }
    }

    private double totalAbutments = 0;
    public double TotalAbutments
    {
        get => totalAbutments;
        set
        {
            totalAbutments = value;
            RaisePropertyChanged(nameof(TotalAbutments));
        }
    }

    private double totalOrders = 0;
    public double TotalOrders
    {
        get => totalOrders;
        set
        {
            totalOrders = value;
            RaisePropertyChanged(nameof(TotalOrders));
        }
    }

    private double totalOrdersToday = 0;
    public double TotalOrdersToday
    {
        get => totalOrdersToday;
        set
        {
            totalOrdersToday = value;
            RaisePropertyChanged(nameof(TotalOrdersToday));
        }
    }

    private double totalOrdersLeftOvers = 0;
    public double TotalOrdersLeftOvers
    {
        get => totalOrdersLeftOvers;
        set
        {
            totalOrdersLeftOvers = value;
            RaisePropertyChanged(nameof(TotalOrdersLeftOvers));
        }
    }


    private double totalUnitsFinal = 0;
    public double TotalUnitsFinal
    {
        get => totalUnitsFinal;
        set
        {
            totalUnitsFinal = value;
            RaisePropertyChanged(nameof(TotalUnitsFinal));
        }
    }

    private double totalUnitsTodayFinal = 0;
    public double TotalUnitsTodayFinal
    {
        get => totalUnitsTodayFinal;
        set
        {
            totalUnitsTodayFinal = value;
            RaisePropertyChanged(nameof(TotalUnitsTodayFinal));
        }
    }

    private double totalUnitsLeftOverFinal = 0;
    public double TotalUnitsLeftOverFinal
    {
        get => totalUnitsLeftOverFinal;
        set
        {
            totalUnitsLeftOverFinal = value;
            RaisePropertyChanged(nameof(TotalUnitsLeftOverFinal));
        }
    }



    private double totalCrownsFinal = 0;
    public double TotalCrownsFinal
    {
        get => totalCrownsFinal;
        set
        {
            totalCrownsFinal = value;
            RaisePropertyChanged(nameof(TotalCrownsFinal));
        }
    }

    private double totalAbutmentsFinal = 0;
    public double TotalAbutmentsFinal
    {
        get => totalAbutmentsFinal;
        set
        {
            totalAbutmentsFinal = value;
            RaisePropertyChanged(nameof(TotalAbutmentsFinal));
        }
    }

    private double totalOrdersFinal = 0;
    public double TotalOrdersFinal
    {
        get => totalOrdersFinal;
        set
        {
            totalOrdersFinal = value;
            RaisePropertyChanged(nameof(TotalOrdersFinal));
        }
    }

    private double totalOrdersTodayFinal = 0;
    public double TotalOrdersTodayFinal
    {
        get => totalOrdersTodayFinal;
        set
        {
            totalOrdersTodayFinal = value;
            RaisePropertyChanged(nameof(TotalOrdersTodayFinal));
        }
    }

    private double totalOrdersLeftOversFinal = 0;
    public double TotalOrdersLeftOversFinal
    {
        get => totalOrdersLeftOversFinal;
        set
        {
            totalOrdersLeftOversFinal = value;
            RaisePropertyChanged(nameof(TotalOrdersLeftOversFinal));
        }
    }
    private Visibility totalUnitsTodaySameAsAllTimeTotal = Visibility.Visible;
    public Visibility TotalUnitsTodaySameAsAllTimeTotal
    {
        get => totalUnitsTodaySameAsAllTimeTotal;
        set
        {
            totalUnitsTodaySameAsAllTimeTotal = value;
            RaisePropertyChanged(nameof(TotalUnitsTodaySameAsAllTimeTotal));
        }
    }

    private Visibility totalOrdersTodaySameAsAllTimeTotal = Visibility.Visible;
    public Visibility TotalOrdersTodaySameAsAllTimeTotal
    {
        get => totalOrdersTodaySameAsAllTimeTotal;
        set
        {
            totalOrdersTodaySameAsAllTimeTotal = value;
            RaisePropertyChanged(nameof(TotalOrdersTodaySameAsAllTimeTotal));
        }
    }


    #endregion units

    private List<CheckedOutCasesModel> sentOutCasesModelRAW = [];
    public List<CheckedOutCasesModel> SentOutCasesModelRAW
    {
        get => sentOutCasesModelRAW;
        set
        {
            sentOutCasesModelRAW = value;
            RaisePropertyChanged(nameof(SentOutCasesModelRAW));
        }
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

    private List<CheckedOutCasesModel> sentOutCasesModelFinal = [];
    public List<CheckedOutCasesModel> SentOutCasesModelFinal
    {
        get => sentOutCasesModelFinal;
        set
        {
            sentOutCasesModelFinal = value;
            RaisePropertyChanged(nameof(SentOutCasesModelFinal));
        }
    }

    private string search = "";
    public string Search
    {
        get => search;
        set
        {
            search = value;
            RaisePropertyChanged(nameof(Search));
            if (!string.IsNullOrEmpty(value))
                Filter();
            else
                SentOutCasesModelFinal = SentOutCasesModel;
        }
    }

    private Visibility showInfoPanel = Visibility.Hidden;
    public Visibility ShowInfoPanel
    {
        get => showInfoPanel;
        set
        {
            showInfoPanel = value;
            RaisePropertyChanged(nameof(ShowInfoPanel));
        }
    }

    private Visibility hasImages = Visibility.Collapsed;
    public Visibility HasImages
    {
        get => hasImages;
        set
        {
            hasImages = value;
            RaisePropertyChanged(nameof(HasImages));
        }
    }

    private string selectedOrderID = "";
    public string SelectedOrderID
    {
        get => selectedOrderID;
        set
        {
            selectedOrderID = value;
            RaisePropertyChanged(nameof(SelectedOrderID));
        }
    }

    private string selectedItems = "";
    public string SelectedItems
    {
        get => selectedItems;
        set
        {
            selectedItems = value;
            RaisePropertyChanged(nameof(SelectedItems));
        }
    }

    private string selectedComment = "";
    public string SelectedComment
    {
        get => selectedComment;
        set
        {
            selectedComment = value;
            RaisePropertyChanged(nameof(SelectedComment));
        }
    }

    private string selectedExtraComment = "";
    public string SelectedExtraComment
    {
        get => selectedExtraComment;
        set
        {
            selectedExtraComment = value;
            RaisePropertyChanged(nameof(SelectedExtraComment));
        }
    }

    private bool getOrderImageButtonEnabled = true;
    public bool GetOrderImageButtonEnabled
    {
        get => getOrderImageButtonEnabled;
        set
        {
            getOrderImageButtonEnabled = value;
            RaisePropertyChanged(nameof(GetOrderImageButtonEnabled));
        }
    }

    private List<CheckedOutCaseImagesModel> orderImages = [];
    public List<CheckedOutCaseImagesModel> OrderImages
    {
        get => orderImages;
        set
        {
            orderImages = value;
            RaisePropertyChanged(nameof(OrderImages));
        }
    }

    private CheckedOutCaseImagesModel selectedOrderImage;
    public CheckedOutCaseImagesModel SelectedOrderImage
    {
        get => selectedOrderImage;
        set
        {
            selectedOrderImage = value;
            RaisePropertyChanged(nameof(SelectedOrderImage));
            ConvertImageHashBackToImage();
        }
    }


    private BitmapImage convertedImage;
    public BitmapImage ConvertedImage
    {
        get => convertedImage;
        set
        {
            convertedImage = value;
            RaisePropertyChanged(nameof(ConvertedImage));
        }
    }

    private Visibility showImagePanel = Visibility.Hidden;
    public Visibility ShowImagePanel
    {
        get => showImagePanel;
        set
        {
            showImagePanel = value;
            RaisePropertyChanged(nameof(ShowImagePanel));
        }
    }

    #endregion Properties

    public RelayCommand FilterCommand { get; set; }
    public RelayCommand ClearFilterCommand { get; set; }
    public RelayCommand CloseOrderInfoPanelCommand { get; set; }
    public RelayCommand OpenOrderInfoPanelCommand { get; set; }
    public RelayCommand GetOrderImagesCommand { get; set; }
    public RelayCommand CloseImagePanelCommand { get; set; }


    public UserPanelViewModel()
    {
        Instance = this;
        mainViewModel = MainViewModel.Instance;

        mainViewModel.UserPanelViewModels.Add(this);

        _orderTimer = new System.Timers.Timer(60000);
        _orderTimer.Elapsed += OrderTimer_Elapsed;
        _orderTimer.Start();

        _startTimer = new System.Timers.Timer(1000);
        _startTimer.Elapsed += StartTimer_Elapsed;
        _startTimer.Start();

        _periodicTimer = new System.Timers.Timer(2000);
        _periodicTimer.Elapsed += PeriodicTimer_Elapsed;
        _periodicTimer.Start();

        Lang = LoginViewModel.Instance.Lang;

        Language = (string)Lang["language"];


        SentOutCasesModelRAW = MainViewModel.Instance.SentOutCasesModel;

        FilterCommand = new RelayCommand(o => Filter());
        ClearFilterCommand = new RelayCommand(o => ClearFilter());
        CloseOrderInfoPanelCommand = new RelayCommand(o => CloseOrderInfoPanel());
        OpenOrderInfoPanelCommand = new RelayCommand(o => OpenOrderInfoPanel(o));
        GetOrderImagesCommand = new RelayCommand(o => GetOrderImages());
        CloseImagePanelCommand = new RelayCommand(o => CloseImagePanel());
    }

    private void CloseImagePanel()
    {
        ShowImagePanel = Visibility.Collapsed;
        SelectedOrderImage = null;
        SelectedOrderImage = null;
        ConvertedImage = null;
        GetOrderImageButtonEnabled = true;
    }

    private void ConvertImageHashBackToImage()
    {
        if (SelectedOrderImage is not null)
            ShowImagePanel = Visibility.Visible;
    }

    private async void GetOrderImages()
    {
        GetOrderImageButtonEnabled = false;
        OrderImages = [];
        if (await GetOrderImagesFromAPI())
            HasImages = Visibility.Collapsed;
        else
            GetOrderImageButtonEnabled = true;
    }

    private async Task<bool> GetOrderImagesFromAPI()
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var http = new HttpClient(handler);
        http.DefaultRequestHeaders.Add("DeviceId", MainViewModel.Instance.DeviceId);

        List<CheckedOutCaseImagesModel> imageModelList = [];

        try
        {
            string result = await http.GetStringAsync($"https://{MainViewModel.Instance.ServerAddress}:10113/api/statsgetcheckedoutcaseimages/{SelectedOrderID}");
            imageModelList = JsonConvert.DeserializeObject<List<CheckedOutCaseImagesModel>>(result)!;
        }
        catch (Exception ex)
        {
            MainViewModel.Instance.AddToDebug("#99: " + ex.Message);
            return false;
        }
        http.Dispose();
        handler.Dispose();

        OrderImages = imageModelList;
        return true;
    }

    private void CloseOrderInfoPanel()
    {
        ShowInfoPanel = Visibility.Hidden;
        OrderImages = [];
    }

    public void OpenOrderInfoPanel(object obj)
    {
        if (obj is not null)
        {
            if (obj is CheckedOutCasesModel model)
            {
                SelectedOrderID = model.OrderID!;
                SelectedItems = model.Items!;

                SelectedComment = model.CommentIn3Shape!;
                SelectedExtraComment = model.Comment!;

                if (model.HasImage == "1")
                    HasImages = Visibility.Visible;
                else
                    HasImages = Visibility.Collapsed;
            }
        }
        ShowInfoPanel = Visibility.Visible;
    }

    private void PeriodicTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        SortOrders();
    }

    private void StartTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Debug.WriteLine("UserPanelViewModel DesignerID: " + DesignerID);
        Debug.WriteLine("UserPanelViewModel AccessLevel: " + MainViewModel.Instance.AccessLevel);
        if (MainViewModel.Instance.AccessLevel.Equals("both", StringComparison.CurrentCultureIgnoreCase))
            DesignerName = mainViewModel.DesignersModel.FirstOrDefault(x => x.DesignerID == DesignerID)?.FriendlyName!;
        else
            DesignerName = MainViewModel.Instance.SiteID;

        _ = GetTheOrderInfos();
        _startTimer.Stop();
    }

    private void OrderTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (!mainViewModel.ServerInfoModel.ServerIsWritingDatabase & mainViewModel.ServerIsOnline)
        {
            _ = GetTheOrderInfos();
        }
    }

    private void Filter()
    {
        if (!string.IsNullOrEmpty(Search))
            SentOutCasesModelFinal = SentOutCasesModel.Where(x =>
                x.OrderID!.Contains(Search, StringComparison.CurrentCultureIgnoreCase) ||
                x.Items!.Contains(Search, StringComparison.CurrentCultureIgnoreCase) ||
                x.CommentIn3Shape!.Contains(Search, StringComparison.CurrentCultureIgnoreCase)
            ).ToList();
    }

    private void ClearFilter()
    {
        Search = "";
    }

    private async Task GetTheOrderInfos()
    {
        //var handler = new HttpClientHandler()
        //{
        //    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        //};

        //var http = new HttpClient(handler);
        //http.DefaultRequestHeaders.Add("DeviceId", mainViewModel.DeviceId);
        SentOutCasesModelRAW = MainViewModel.Instance.SentOutCasesModel;

        Debug.WriteLine("Getting info for " + DesignerID);
        List<CheckedOutCasesModel> modelList = SentOutCasesModelRAW.Where(x => x.Designer == DesignerID).ToList();
        List<CheckedOutCasesModel> sortedModelList = [];

        try
        {
            //string result = await http.GetStringAsync($"https://{mainViewModel.ServerAddress}:10113/api/statscheckedoutcases/{DesignerID}");
            //modelList = JsonConvert.DeserializeObject<List<CheckedOutCasesModel>>(result)!;

            Task.Run(() =>
            {
                TotalAbutments = 0;
                TotalCrowns = 0;
                TotalOrders = 0;
                TotalOrdersToday = 0;
                TotalUnits = 0;
                TotalUnitsToday = 0;
                TotalOrdersLeftOvers = 0;
                TotalUnitsLeftOver = 0;

                foreach (var model in modelList)
                {
                    model.RedoCaseComment = (string)Lang["redoCaseComment"];
                    model.RushCaseComment = (string)Lang["rushCaseComment"];
                    model.RushForMorningComment = (string)Lang["rushForMorningComment"];
                    model.OrderDesignedComment = (string)Lang["orderDesignedComment"];
                    model.ScrewRetainedCaseComment = (string)Lang["screwRetainedCaseComment"];

                    model.OriginalSentOn = model.SentOn;

                    if (model.TotalUnits!.Length == 1)
                        model.TotalUnitsWithPrefixZero = "0" + model.TotalUnits;
                    else
                        model.TotalUnitsWithPrefixZero = model.TotalUnits;

                    if (model.Crowns == "0")
                        model.Crowns = "";
                    if (model.Abutments == "0")
                        model.Abutments = "";
                    if (model.Models == "0")
                        model.Models = "";
                    else
                        model.Models = "🗸";

                    if (model.OriginalSentOn == DateTime.Now.ToString("MM-dd-yyyy"))
                        model.SentOn = $"z{(string)Lang["today"]}";
                    if (model.OriginalSentOn == DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy"))
                        model.SentOn = $"9{(string)Lang["yesterday"]}";

                    if (model.SentOn != $"z{(string)Lang["today"]}" || model.SentOn != $"9{(string)Lang["yesterday"]}")
                    {
                        if (DateTime.TryParse(model.SentOn, out DateTime parsedSentOn))
                        {
                            DateTime sentOn = DateTime.ParseExact(model.SentOn, "MM-dd-yyyy", CultureInfo.InvariantCulture);

                            if (sentOn > DateTime.Now.AddYears(-2))
                            //if (DateTime.TryParse(model.SentOn, out DateTime sentOn))
                            {
                                string dayName = sentOn.ToString("dddd");

                                dayName = dayName switch
                                {
                                    "星期一" => $"2{(string)Lang["monday"]}",
                                    "星期二" => $"3{(string)Lang["tuesday"]}",
                                    "星期三" => $"4{(string)Lang["wednesday"]}",
                                    "星期四" => $"5{(string)Lang["thursday"]}",
                                    "星期五" => $"6{(string)Lang["friday"]}",
                                    "星期六" => $"7{(string)Lang["saturday"]}",
                                    "星期日" => $"8{(string)Lang["sunday"]}",
                                    "Monday" => $"2{(string)Lang["monday"]}",
                                    "Tuesday" => $"3{(string)Lang["tuesday"]}",
                                    "Wednesday" => $"4{(string)Lang["wednesday"]}",
                                    "Thursday" => $"5{(string)Lang["thursday"]}",
                                    "Friday" => $"6{(string)Lang["friday"]}",
                                    "Saturday" => $"7{(string)Lang["saturday"]}",
                                    "Sunday" => $"8{(string)Lang["sunday"]}",
                                    _ => dayName,
                                };
                                model.SentOn = dayName;
                            }
                        }
                    }

                    model.OriginalSentOnForChangedSentOn = model.SentOn;

                    model.IconImage = GetIcon(model.ScanSource!, model.CommentIcon!);

                    if (Language == "Chinese")
                        model.Items = Translate(model.Items!);

                    model.Items = model.Items!.Replace($"{(string)Lang["unsectionedModel"]}, {(string)Lang["antagonistModel"]}", (string)Lang["model"])
                                              .Replace((string)Lang["unsectionedModel"], (string)Lang["model"])
                                              .Replace((string)Lang["antagonistModel"], (string)Lang["model"]);

                    model.CommentIn3Shape = model.CommentIn3Shape!.Trim()
                                                                 .Replace("!", "")
                                                                 .Replace("Thanks", "")
                                                                 .Replace("Thank you", "")
                                                                 .Replace("Thank You", "")
                                                                 .Replace("[Converted From FDI]", "")
                                                                 .Trim();

                    model.CommentIn3Shape = LineBreaksRegEx().Replace(model.CommentIn3Shape, string.Empty);

                    if (model.CommentIn3Shape!.Contains(" redo", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("redo ", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("re do", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Equals("redo", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("remake ", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains(" remake", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Equals("remake", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("return to lab", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("returned to lab", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("open margin", StringComparison.CurrentCultureIgnoreCase))
                    {
                        model.CommentColor = "Maroon";
                        model.Redo = "1";
                    }

                    if (model.CommentIn3Shape!.Contains("screw retained", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("access hole", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("screwmented", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("screwret", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("srewret", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("screw access", StringComparison.CurrentCultureIgnoreCase) ||
                        model.OrderID!.EndsWith("-SCR", StringComparison.CurrentCultureIgnoreCase) ||
                        model.OrderID!.EndsWith("-SRC", StringComparison.CurrentCultureIgnoreCase) ||
                        model.OrderID!.EndsWith("-ACH", StringComparison.CurrentCultureIgnoreCase))
                    {
                        model.ScrewRetained = true;
                        model.CommentColor = "#b90ffa";
                    }

                    if (model.CommentIn3Shape!.Contains(" rush", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("rush ", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Equals("rush", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("expedite ", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains(" expedite", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Equals("expedite", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains("asap ", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Contains(" asap", StringComparison.CurrentCultureIgnoreCase) ||
                        model.CommentIn3Shape!.Equals("asap", StringComparison.CurrentCultureIgnoreCase) ||
                        model.OrderID!.EndsWith("-ASAP", StringComparison.CurrentCultureIgnoreCase) ||
                        model.OrderID!.EndsWith("-RUSH", StringComparison.CurrentCultureIgnoreCase))
                    {
                        model.Rush = "1";
                        model.CommentColor = "Crimson";
                        model.SentOn = $"0{(string)Lang["rush"]}";
                    }

                    // clearing comment, if it's a standard iTero comment
                    if (model.CommentIn3Shape.Contains("Exported from iTero system"))
                        model.CommentIn3Shape = "";
                    else if (!string.IsNullOrEmpty(model.CommentIn3Shape))
                    {
                        string comment = model.CommentIn3Shape;
                        string commentCleaned = "";
                        foreach (var line in comment.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!line.StartsWith("This case is a copy of") || !line.StartsWith("Renamed file of:"))
                                commentCleaned += line + Environment.NewLine;
                        }
                        if (string.IsNullOrEmpty(commentCleaned))
                            commentCleaned = model.CommentIn3Shape;

                        model.CommentIn3Shape = char.ToUpper(commentCleaned[0]) + commentCleaned.Substring(1);
                    }



                    if (model.CommentIcon == "7")
                    {
                        model.Crowns = "";
                        model.Abutments = "";
                        model.Models = "";
                        model.TotalUnits = "0";
                        model.TotalUnitsWithPrefixZero = "00";
                        model.SentOn = $"0{(string)Lang["designReady"]}";
                    }

                    bool noScanFile = false;
                    if (model.Comment is not null)
                    {
                        if (model.Comment.StartsWith("This case is NOT in"))
                        {
                            model.Crowns = "";
                            model.Abutments = "";
                            model.Models = "";
                            model.TotalUnits = "0";
                            model.TotalUnitsWithPrefixZero = "00";
                            model.CommentColor = "Gray";
                            model.SentOn = (string)Lang["noScanFile"];
                            noScanFile = true;
                        }
                    }

                    if (model.CommentIcon == "8")
                    {
                        model.CommentColor = "Blue";
                        model.SentOn = $"1{(string)Lang["change"]}";
                    }

                    #region Counting units

                    if (!string.IsNullOrEmpty(model.Crowns))
                    {
                        _ = int.TryParse(model.Crowns, out int crowns);

                        TotalCrowns += crowns;
                        TotalUnits += crowns;

                        if (model.OriginalSentOn!.Equals(DateTime.Now.ToString("MM-dd-yyyy")) ||
                            (model.OriginalSentOn!.Equals(DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy")) &&
                                DateTime.Now.Hour < 5))
                            TotalUnitsToday += crowns;
                    }

                    if (!string.IsNullOrEmpty(model.Abutments))
                    {
                        _ = int.TryParse(model.Abutments, out int abutments);

                        TotalAbutments += abutments;
                        TotalUnits += abutments;

                        if (model.OriginalSentOn!.Equals(DateTime.Now.ToString("MM-dd-yyyy")) ||
                            (model.OriginalSentOn!.Equals(DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy")) &&
                                DateTime.Now.Hour < 5))
                            TotalUnitsToday += abutments;
                    }


                    if (model.OriginalSentOn!.Equals(DateTime.Now.ToString("MM-dd-yyyy")) ||
                            (model.OriginalSentOn!.Equals(DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy")) &&
                                DateTime.Now.Hour < 5))
                    {
                        if (!noScanFile)
                            TotalOrdersToday++;
                    }

                    if (!noScanFile)
                        TotalOrders++;


                    if (TotalOrders == TotalOrdersToday || TotalOrdersToday == 0)
                        TotalOrdersTodaySameAsAllTimeTotal = Visibility.Hidden;
                    else
                        TotalOrdersTodaySameAsAllTimeTotal = Visibility.Visible;

                    if (TotalUnits == TotalUnitsToday || TotalUnitsToday == 0)
                        TotalUnitsTodaySameAsAllTimeTotal = Visibility.Hidden;
                    else
                        TotalUnitsTodaySameAsAllTimeTotal = Visibility.Visible;

                    #endregion Counting units

                    if (model.Rush == "1")
                    {
                        model.CommentColor = "Crimson";
                        model.SentOn = $"0{(string)Lang["rush"]}";
                    }



                    if (Language == "Chinese")
                        model.Comment = TranslateComment(model.Comment!);
                }

            }).Wait();

            TotalOrdersLeftOvers = TotalOrders - TotalOrdersToday;


            TotalUnitsLeftOver = TotalUnits - TotalUnitsToday;

            _ = Task.Run(StartPresentingUnitNumbers);

            if (mainViewModel.OrderByIndex == 0)
            {
                sortedModelList = [.. modelList.OrderBy(x => x.SentOn).ThenByDescending(x => x.Rush).ThenBy(x => x.CommentIcon).ThenByDescending(x => x.TotalUnitsWithPrefixZero)];
            }
            else if (mainViewModel.OrderByIndex == 1)
            {
                sortedModelList = [.. modelList.OrderBy(x => x.SentOn).ThenByDescending(x => x.Rush).ThenBy(x => x.CommentIcon).ThenBy(x => x.OrderID)];
            }




            SentOutCasesModel = sortedModelList;
            if (string.IsNullOrEmpty(Search))
                SentOutCasesModelFinal = sortedModelList;
        }
        catch (Exception ex)
        {
            mainViewModel.AddToDebug("#4: " + ex.Message);
        }
        //http.Dispose();
        //handler.Dispose();

        SortOrders();
    }


    private void SortOrders()
    {
        if (SentOutCasesModel is not null)
        {
            List<CheckedOutCasesModel>? modelList = SentOutCasesModel;
            List<CheckedOutCasesModel>? sortedModelList = [];

            if (mainViewModel.OrderByIndex == 0)
            {
                if (modelList is not null)
                {
                    sortedModelList = [.. modelList.OrderBy(x => x.SentOn).ThenByDescending(x => x.Rush).ThenBy(x => x.CommentIcon).ThenByDescending(x => x.TotalUnitsWithPrefixZero)];
                    SentOutCasesModel = sortedModelList;
                    SentOutCasesModelFinal = sortedModelList;
                }
            }
            else if (mainViewModel.OrderByIndex == 1)
            {
                if (modelList is not null)
                {
                    sortedModelList = [.. modelList.OrderBy(x => x.SentOn).ThenByDescending(x => x.Rush).ThenBy(x => x.CommentIcon).ThenByDescending(x => x.OrderID)];
                    SentOutCasesModel = sortedModelList;
                    SentOutCasesModelFinal = sortedModelList;
                }
            }

        }
    }

    private static string GetIcon(string ScanSource, string commentIcon)
    {
        if (commentIcon == "7") return "/Images/crown.png";

        return ScanSource switch
        {
            "ss3ShapeDesktopScanner" => "/Images/i10.png",
            "ss3SE4" => "/Images/i33.png",
            "ss3SE3" => "/Images/i33.png",
            "ss3SE2" => "/Images/i33.png",
            "ss3SD2000" => "/Images/i20.png",
            "ss3SD1000" => "/Images/i20.png",
            "ss3SD900" => "/Images/i20.png",
            "ss3SD810" => "/Images/i20.png",
            "ss3SD800" => "/Images/i20.png",
            "ss3SD700" => "/Images/i20.png",
            _ => "/Images/trios_new.png",
        };
    }

    private static string TranslateComment(string text)
    {
        text = text.Replace("Someone moved this case manually here.", "有人手动将此案例移至此处");
        text = text.Replace("This case is NOT in the export folder!!!", "此案例不在导出文件夹中！ 忽略这个案例");
        text = text.Replace("This case were sent to both designer!", "这个案子发给了两位设计师");
        text = text.Replace("The design needs to change!", "设计需要改变！");
        return text;
    }


    private static string Translate(string text)
    {
        text = text.Replace("Unsectioned model", "未分割模型");
        text = text.Replace("Antagonist model", "对合模型");
        text = text.Replace("Sectioned (die ditched) model", "分割模型");
        text = text.Replace("Soft tissue", "软组织");
        text = text.Replace("Die", "代型");

        text = text.Replace("Temporary on prepared model", "已制备模型上的临时冠");
        text = text.Replace("Anatomy bridge with gingiva", "解剖牙桥 含牙龈");
        text = text.Replace("Crown with gingiva", "牙冠 含牙龈");
        text = text.Replace("Anatomical coping", "解剖型内冠");


        text = text.Replace("Anatomy bridge", "解剖牙桥");
        text = text.Replace("Frame bridge", "框架桥");

        text = text.Replace("Temporary Crown", "临时冠");

        text = text.Replace("Anatomical Abutment", "解剖型基台");
        text = text.Replace("Post and Core", "桩核");
        text = text.Replace("Inlay", "嵌体");
        text = text.Replace("Onlay", "高嵌体");
        text = text.Replace("Abutment", "基台");
        text = text.Replace("Screw Retained Crown", "螺丝固位冠");
        text = text.Replace("Veneer", "贴面");

        text = text.Replace("Coping", "内冠");
        text = text.Replace("Crown", "牙冠");

        return text;
    }

    private async void StartPresentingUnitNumbers()
    {
        if (TotalUnitsFinal != TotalUnits)
            await CountUp_TotalUnits(TotalUnits);
        if (TotalCrownsFinal != TotalCrowns)
            await CountUp_TotalCrowns(TotalCrowns);
        if (TotalAbutmentsFinal != TotalAbutments)
            await CountUp_TotalAbutments(TotalAbutments);
        if (TotalOrdersFinal != TotalOrders)
            await CountUp_TotalOrders(TotalOrders);
        if (TotalUnitsLeftOverFinal != TotalUnitsLeftOver)
            await CountUp_TotalUnitsLeftOver(TotalUnitsLeftOver);
        if (TotalOrdersLeftOversFinal != TotalOrdersLeftOvers)
            await CountUp_TotalOrdersLeftOvers(TotalOrdersLeftOvers);
        if (TotalUnitsTodayFinal != TotalUnitsToday)
            await CountUp_TotalUnitsToday(TotalUnitsToday);
        if (TotalOrdersTodayFinal != TotalOrdersToday)
            await CountUp_TotalOrdersToday(TotalOrdersToday);
    }

    #region CoutUp functions
    private async Task CountUp_TotalUnits(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalUnitsFinal = i;
            Thread.Sleep(10);
        }
    }

    private async Task CountUp_TotalCrowns(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalCrownsFinal = i;
            Thread.Sleep(10);
        }
    }

    private async Task CountUp_TotalAbutments(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalAbutmentsFinal = i;
            Thread.Sleep(10);
        }
    }

    private async Task CountUp_TotalOrders(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalOrdersFinal = i;
            Thread.Sleep(10);
        }
    }

    private async Task CountUp_TotalUnitsLeftOver(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalUnitsLeftOverFinal = i;
            Thread.Sleep(10);
        }
    }

    private async Task CountUp_TotalOrdersLeftOvers(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalOrdersLeftOversFinal = i;
            Thread.Sleep(10);
        }
    }

    private async Task CountUp_TotalUnitsToday(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalUnitsTodayFinal = i;
            Thread.Sleep(10);
        }
    }

    private async Task CountUp_TotalOrdersToday(double Max)
    {
        for (int i = 0; i <= Max; i++)
        {
            TotalOrdersTodayFinal = i;
            Thread.Sleep(10);
        }
    }


    #endregion CoutUp functions

    [GeneratedRegex(@"^\s+$[\r\n]*", RegexOptions.Multiline)]
    private static partial Regex LineBreaksRegEx();
}
