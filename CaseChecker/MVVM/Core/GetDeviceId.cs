using System.Security.Cryptography;
using System.Text;
using System.Management;
using System.Windows;
using System.IO;
using System.Reflection;

namespace CaseChecker.MVVM.Core;

public class GetDeviceId
{
    public static string GetDeviceID()
    {
        string? id = null;
        
        try
        {
            id = MotherboardInfo.SerialNumber;
        }
        catch (Exception) 
        { 
        }

        id ??= Environment.MachineName;

        return CreateMD5(id);
    }

    public static string CreateMD5(string input)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static string ReadDeviceInfo()
    {
        StringBuilder sb = new();

        sb.AppendLine("{");
        sb.AppendLine("\"Model\" : \"" + MotherboardInfo.Product + "\", ");
        sb.AppendLine("\"Manufacturer\" : \"" + MotherboardInfo.Manufacturer + "\", ");
        sb.AppendLine("\"Name\" : \"" + Environment.MachineName + "\", ");
        sb.AppendLine("\"OSVersion\" : \"" + OSVersion() + "\", ");
        sb.AppendLine("\"Idiom\" : \"" + "Desktop" + "\", ");
        sb.AppendLine("\"Platform\" : \"" + "WPF" + "\", ");
        sb.AppendLine("\"VirtualDevice\" : \"False\", ");
        sb.AppendLine("\"AppVersion\" : \"" + GetAppVersion() + "\", ");
        sb.AppendLine("\"CPU\" : \"" + GetCpu() + "\", ");
        sb.AppendLine("\"RAM\" : \"" + GetRAM() + "\", ");
        sb.AppendLine("\"LastLogin\" : \"" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC\"");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GetRAM()
    {
        try
        {
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            long installedMemory = gcMemoryInfo.TotalAvailableMemoryBytes;
            var physicalMemory = installedMemory / 1048576.0 / 1024;
            return Math.Round(physicalMemory).ToString() + "GiB";
        }
        catch (Exception)
        {

        }
        return "";
    }

    public static string GetCpu()
    { 
        try
        {
            ManagementObjectSearcher mosProcessor = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            string Procname = null;
            string Core = null;
            foreach (ManagementObject moProcessor in mosProcessor.Get())
            {
                if (moProcessor["name"] != null)
                {
                    Procname = moProcessor["name"].ToString();
                }

            }

            Procname = Procname
               .Replace("(TM)", "™")
               .Replace("(tm)", "™")
               .Replace("(R)", "®")
               .Replace("(r)", "®")
               .Replace("(C)", "©")
               .Replace("(c)", "©")
               .Replace("    ", " ")
               .Replace("  ", " ");

            return Procname;
        }
        catch (Exception) 
        {
        }

        return "";
    }

    public static string GetAppVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("version.txt"));
        string versionResult = "";
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new(stream))
        {
            versionResult = reader.ReadToEnd();
        }

        return versionResult;
    }

    public static string OSVersion()
    {
        try
        {
            string r = "";
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectCollection information = searcher.Get();
            if (information != null)
            {
                foreach (ManagementObject obj in information.Cast<ManagementObject>())
                {
                    r = obj["Caption"].ToString() + " - " + obj["OSArchitecture"].ToString();
                }
            }
            r = r.Replace("NT 5.1.2600", "XP");
            r = r.Replace("NT 5.2.3790", "Server 2003");
            return r.Replace("Microsoft", "").Trim();
        }
        catch (Exception)
        {
        }
        return "Windows";
    }
}


static public class MotherboardInfo
{
    private static ManagementObjectSearcher baseboardSearcher = new("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
       
    static public string Product
    {
        get
        {
            try
            {
                foreach (ManagementObject queryObj in baseboardSearcher.Get().Cast<ManagementObject>())
                {
                    return queryObj["Product"].ToString();
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }

    static public string Manufacturer
    {
        get
        {
            try
            {
                foreach (ManagementObject queryObj in baseboardSearcher.Get().Cast<ManagementObject>())
                {
                    return queryObj["Manufacturer"].ToString();
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }

    static public string SerialNumber
    {
        get
        {
            try
            {
                foreach (ManagementObject queryObj in baseboardSearcher.Get().Cast<ManagementObject>())
                {
                    return queryObj["SerialNumber"].ToString();
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }

    
}
