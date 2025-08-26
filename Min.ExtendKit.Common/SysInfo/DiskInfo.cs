using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.SysInfo;

/// <summary>
/// 提供磁盘相关的信息封装。
/// </summary>
[SupportedOSPlatform("windows")]
public static class DiskInfo
{
    /// <summary>
    /// 获取指定驱动器的剩余可用空间（字节）。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>剩余可用空间，单位字节。</returns>
    public static long GetAvailableFreeSpace(string driveLetter = "C") => new DriveInfo(driveLetter).AvailableFreeSpace;

    /// <summary>
    /// 获取指定驱动器已使用空间（字节）。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>已使用空间（字节），驱动器不可用时返回 0。</returns>
    public static long GetUsedSpace(string driveLetter = "C")
    {
        try
        {
            var drive = new DriveInfo(driveLetter);
            return drive.IsReady ? drive.TotalSize - drive.AvailableFreeSpace : 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 获取指定驱动器的总容量（字节）。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>总容量，单位字节。</returns>
    public static long GetTotalSize(string driveLetter = "C") => new DriveInfo(driveLetter).TotalSize;

    /// <summary>
    /// 获取指定驱动器使用率（百分比）。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>使用率（0–100%），驱动器不可用时返回 0。</returns>
    public static double GetUsageRate(string driveLetter = "C")
    {
        var total = GetTotalSize(driveLetter);
        if (total == 0) return 0;
        var used = GetUsedSpace(driveLetter);
        return Math.Round(used * 100.0 / total, 2);
    }

    /// <summary>
    /// 获取指定驱动器卷标（Label）。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>卷标，驱动器不可用时返回 null。</returns>
    public static string? GetVolumeLabel(string driveLetter = "C")
    {
        try
        {
            var drive = new DriveInfo(driveLetter);
            return drive.IsReady ? drive.VolumeLabel : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取指定驱动器文件系统类型，例如 NTFS、FAT32。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>文件系统类型，驱动器不可用时返回 null。</returns>
    public static string? GetDriveFormat(string driveLetter = "C")
    {
        try
        {
            var drive = new DriveInfo(driveLetter);
            return drive.IsReady ? drive.DriveFormat : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 判断指定驱动器是否就绪。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>就绪返回 true，否则 false。</returns>
    public static bool IsReady(string driveLetter = "C")
    {
        try
        {
            var drive = new DriveInfo(driveLetter);
            return drive.IsReady;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取驱动器类型，例如 Fixed（固定磁盘）、Removable（可移动磁盘）、CDRom 等。
    /// </summary>
    /// <param name="driveLetter">驱动器盘符，例如 "C"。</param>
    /// <returns>驱动器类型，驱动器不可用时返回 null。</returns>
    public static DriveType? GetDriveType(string driveLetter = "C")
    {
        try
        {
            var drive = new DriveInfo(driveLetter);
            return drive.IsReady ? drive.DriveType : null;
        }
        catch
        {
            return null;
        }
    }

}