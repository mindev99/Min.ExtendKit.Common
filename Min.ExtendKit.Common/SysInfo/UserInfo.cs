using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.SysInfo;

/// <summary>
/// 提供用户与环境相关的信息封装。
/// </summary>
[SupportedOSPlatform("windows")]
public static class UserInfo
{
    #region 用户信息

    /// <summary>
    /// 当前登录用户名。
    /// </summary>
    public static string UserName => Environment.UserName;

    /// <summary>
    /// 当前用户的域名（如果在域环境下）。
    /// </summary>
    public static string UserDomainName => Environment.UserDomainName;

    /// <summary>
    /// 当前用户的 SID（安全标识符）。
    /// </summary>
    public static string UserSID
    {
        get
        {
            try
            {
                using System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                return identity.User?.Value ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// 是否为管理员。
    /// </summary>
    public static bool IsAdministrator
    {
        get
        {
            try
            {
                using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }

    #endregion

    #region 环境路径

    /// <summary>
    /// 当前用户主目录路径。
    /// </summary>
    public static string UserProfilePath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    /// <summary>
    /// 当前系统的临时目录路径。
    /// </summary>
    public static string TempPath => System.IO.Path.GetTempPath();

    /// <summary>
    /// 当前用户的文档目录路径。
    /// </summary>
    public static string MyDocumentsPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    /// 当前用户的桌面目录路径。
    /// </summary>
    public static string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    /// <summary>
    /// 获取当前用户的下载目录路径。
    /// </summary>
    public static string DownloadsPath => Path.Combine(UserProfilePath, "Downloads");

    /// <summary>
    /// 当前用户的应用数据目录路径（Roaming）。
    /// </summary>
    public static string AppDataPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    /// 当前用户的本地应用数据目录路径（Local）。
    /// </summary>
    public static string LocalAppDataPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    /// <summary>
    /// 当前程序所在目录。
    /// </summary>
    public static string CurrentDirectory => Environment.CurrentDirectory;

    /// <summary>
    /// 当前正在执行的程序路径。
    /// </summary>
    public static string ExecutablePath => System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;

    #endregion

    #region 网络信息

    /// <summary>
    /// 获取本机 IPv4 地址列表。
    /// </summary>
    public static string[] LocalIPv4
    {
        get
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(a => a.Address.ToString())
                .ToArray();
        }
    }

    /// <summary>
    /// 获取所有活动网络接口的 MAC 地址。
    /// </summary>
    public static string[] MACAddresses
    {
        get
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                .Select(n => n.GetPhysicalAddress().ToString())
                .ToArray();
        }
    }

    #endregion

    #region 环境变量

    /// <summary>
    /// 系统 PATH 环境变量，用于查找可执行程序路径。
    /// </summary>
    public static string PATH => Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

    /// <summary>
    /// 系统临时目录环境变量，通常用于存放临时文件。
    /// </summary>
    public static string TEMP => Environment.GetEnvironmentVariable("TEMP") ?? string.Empty;

    /// <summary>
    /// 当前用户的应用数据目录环境变量（Roaming），通常用于存放用户配置文件。
    /// </summary>
    public static string APPDATA => Environment.GetEnvironmentVariable("APPDATA") ?? string.Empty;

    #endregion

}