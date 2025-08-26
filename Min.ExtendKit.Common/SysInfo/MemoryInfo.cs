using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Min.ExtendKit.Common.Core.Struct;
using Min.ExtendKit.Common.Core.Win32.API;

namespace Min.ExtendKit.Common.SysInfo;

/// <summary>
/// 提供内存相关的信息封装。
/// </summary>
[SupportedOSPlatform("windows")]
public class MemoryInfo
{
    #region 物理内存

    /// <summary>
    /// 总物理内存（字节）。
    /// </summary>
    public static ulong TotalPhysicalMemory
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetWindowsTotalMemory();
            return (ulong)GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        }
    }

    /// <summary>
    /// 当前可用物理内存（字节）。
    /// </summary>
    public static ulong AvailablePhysicalMemory
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetWindowsAvailableMemory();
            return (ulong)GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        }
    }

    /// <summary>
    /// 已使用物理内存（字节）。
    /// </summary>
    public static ulong UsedPhysicalMemory => TotalPhysicalMemory - AvailablePhysicalMemory;

    /// <summary>
    /// 物理内存使用率（百分比）。
    /// </summary>
    public static uint PhysicalMemoryLoad => GetWindowsMemory()?.dwMemoryLoad ?? 0;

    #endregion

    #region 虚拟内存

    /// <summary>
    /// 虚拟内存总量（字节）。
    /// </summary>
    public static ulong TotalVirtualMemory => GetWindowsMemory()?.ullTotalVirtual ?? 0;

    /// <summary>
    /// 当前可用虚拟内存大小（字节）。
    /// </summary>
    public static ulong AvailableVirtualMemory => GetWindowsMemory()?.ullAvailVirtual ?? 0;

    /// <summary>
    /// 已使用虚拟内存大小（字节）。
    /// </summary>
    public static ulong UsedVirtualMemory => TotalVirtualMemory - AvailableVirtualMemory;

    #endregion

    #region 内部方法

    /// <summary>
    /// 调用 Windows API 获取系统内存状态。
    /// </summary>
    /// <returns><see cref="MEMORYSTATUSEX"/> 实例，失败返回 <c>null</c>。</returns>
    private static MEMORYSTATUSEX? GetWindowsMemory()
    {
        try
        {
            MEMORYSTATUSEX mem = new();
            mem.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            if (MemoryAPI.GlobalMemoryStatusEx(ref mem))
                return mem;
        }
        catch
        {
            // 可加入 Debug 输出
        }
        return null;
    }

    /// <summary>
    /// 获取 Windows 系统下的物理内存总量（字节）。
    /// </summary>
    /// <remarks>
    /// 内部调用 <c>GlobalMemoryStatusEx</c> API 获取数据。  
    /// 如果调用失败，将返回 <c>0</c>。
    /// </remarks>
    /// <returns>物理内存总量（字节），失败时返回 <c>0</c>。</returns>
    private static ulong GetWindowsTotalMemory()
    {
        MEMORYSTATUSEX memStatus = new();
        if (MemoryAPI.GlobalMemoryStatusEx(ref memStatus))
            return memStatus.ullTotalPhys;
        return 0;
    }

    /// <summary>
    /// 获取 Windows 系统下的可用物理内存大小（字节）。
    /// </summary>
    /// <remarks>
    /// 内部调用 <c>GlobalMemoryStatusEx</c> API 获取数据。  
    /// 如果调用失败，将返回 <c>0</c>。
    /// </remarks>
    /// <returns>可用物理内存大小（字节），失败时返回 <c>0</c>。</returns>
    private static ulong GetWindowsAvailableMemory()
    {
        MEMORYSTATUSEX memStatus = new();
        if (MemoryAPI.GlobalMemoryStatusEx(ref memStatus))
            return memStatus.ullAvailPhys;
        return 0;
    }

    #endregion

}
