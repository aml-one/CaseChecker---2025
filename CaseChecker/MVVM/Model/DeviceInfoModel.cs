namespace CaseChecker.MVVM.Model;

public class DeviceInfoModel
{
    public string? DeviceId { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? Name { get; set; }
    public string? OSVersion { get; set; }
    public string? Idiom { get; set; }
    public string? Platform { get; set; }
    public bool VirtualDevice { get; set; } = false;
    public string? AppVersion { get; set;}
    public string? FriendlyName { get; set;}
    public string? AccessTo { get; set; }
    public string? CPU { get; set; }
    public string? RAM { get; set; }
    public string? LastLogin { get; set; }
    public string? IsItTheHostDevice { get; set; } = "False";
}
