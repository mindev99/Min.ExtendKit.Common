using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.SysInfo;

/// <summary>
/// 提供操作系统相关的信息封装。
/// </summary>
[SupportedOSPlatform("windows")]
public static class OSInfo
{
    #region 系统描述与架构

    /// <summary>
    /// 操作系统描述，例如 "Microsoft Windows 11 专业版"。
    /// </summary>
    /// <remarks>
    /// 该信息来自 <see cref="RuntimeInformation.OSDescription"/>，可用于显示操作系统版本或日志记录。
    /// </remarks>
    public static string Description => RuntimeInformation.OSDescription;

    /// <summary>
    /// 操作系统架构，例如 X64、X86、Arm 或 Arm64。
    /// </summary>
    /// <remarks>
    /// 来自 <see cref="RuntimeInformation.OSArchitecture"/>。
    /// </remarks>
    public static Architecture Architecture => RuntimeInformation.OSArchitecture;

    /// <summary>
    /// 当前操作系统版本号。
    /// </summary>
    public static Version OSVersion => Environment.OSVersion.Version;

    /// <summary>
    /// 操作系统平台，例如 Win32NT。
    /// </summary>
    public static PlatformID Platform => Environment.OSVersion.Platform;

    #endregion

    #region 系统时间

    /// <summary>
    /// 系统启动时间。
    /// </summary>
    /// <remarks>
    /// 通过 <see cref="Environment.TickCount64"/> 计算系统开机时间。
    /// 使用公式：<c>DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount64)</c>。
    /// </remarks>
    public static DateTime BootTime => DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount64);

    /// <summary>
    /// 当前系统时区名称。
    /// </summary>
    public static string TimeZone => TimeZoneInfo.Local.DisplayName;

    #endregion

    #region 系统语言

    /// <summary>
    /// 系统安装语言。
    /// </summary>
    public static string SystemLanguage => CultureInfo.InstalledUICulture.DisplayName;

    /// <summary>
    /// 当前用户界面语言。
    /// </summary>
    public static string UserInterfaceLanguage => CultureInfo.CurrentUICulture.DisplayName;

    #endregion

    #region 系统信息

    /// <summary>
    /// 是否为 64 位操作系统。
    /// </summary>
    public static bool Is64Bit => Environment.Is64BitOperatingSystem;

    /// <summary>
    /// 系统页大小（字节）。
    /// </summary>
    public static int SystemPageSize => Environment.SystemPageSize;

    /// <summary>
    /// 当前计算机名。
    /// </summary>
    public static string MachineName => Environment.MachineName;

    /// <summary>
    /// 完整 Windows SKU 名称，例如 "Windows 11 专业版"。
    /// </summary>
    public static string WindowsSKU
    {
        get
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                if (key != null)
                {
                    return key.GetValue("ProductName")?.ToString() ?? string.Empty;
                }
            }
            catch { }
            return string.Empty;
        }
    }

    #endregion

    #region 会话信息

    /// <summary>
    /// 当前线程管理 ID（不是 Windows 会话 ID）。
    /// </summary>
    public static int SessionId => Environment.CurrentManagedThreadId;

    #endregion
}