using System.Runtime.InteropServices;

namespace Min.ExtendKit.Common.Core.Struct;

/// <summary>
/// Windows 系统内存状态结构体，用于接收 <c>GlobalMemoryStatusEx</c> API 填充的内存信息。
/// </summary>
/// <remarks>
/// - 使用 <c>GlobalMemoryStatusEx</c> 前，调用者必须先设置 <see cref="dwLength"/> 字段为结构体大小。  
/// - 该结构包含物理内存、页面文件、虚拟内存等多种统计数据。  
/// - 所有大小相关字段单位均为字节（除 <see cref="dwMemoryLoad"/> 外）。  
/// </remarks>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct MEMORYSTATUSEX
{
    /// <summary>
    /// 结构体大小（字节）。
    /// 调用 <c>GlobalMemoryStatusEx</c> 前必须赋值为 <c>Marshal.SizeOf(typeof(MEMORYSTATUSEX))</c>。
    /// </summary>
    internal uint dwLength;

    /// <summary>
    /// 内存使用率（百分比，0–100）。
    /// </summary>
    internal uint dwMemoryLoad;

    /// <summary>
    /// 物理内存总量（字节）。
    /// </summary>
    internal ulong ullTotalPhys;

    /// <summary>
    /// 当前可用的物理内存大小（字节）。
    /// </summary>
    internal ulong ullAvailPhys;

    /// <summary>
    /// 系统页面文件的总大小（字节）。
    /// </summary>
    internal ulong ullTotalPageFile;

    /// <summary>
    /// 当前可用的页面文件大小（字节）。
    /// </summary>
    internal ulong ullAvailPageFile;

    /// <summary>
    /// 虚拟内存总量（字节）。
    /// </summary>
    internal ulong ullTotalVirtual;

    /// <summary>
    /// 当前可用的虚拟内存大小（字节）。
    /// </summary>
    internal ulong ullAvailVirtual;

    /// <summary>
    /// 保留字段，始终为 0，供将来扩展使用。
    /// </summary>
    internal ulong ullAvailExtendedVirtual;

    /// <summary>
    /// 初始化结构体，设置 <see cref="dwLength"/> 为正确大小。
    /// </summary>
    internal void Init()
    {
        dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
    }

    /// <summary>
    /// 初始化结构体，设置 <see cref="dwLength"/> 为正确大小。
    /// </summary>
    public MEMORYSTATUSEX()
    {
        dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        dwMemoryLoad = 0;
        ullTotalPhys = 0;
        ullAvailPhys = 0;
        ullTotalPageFile = 0;
        ullAvailPageFile = 0;
        ullTotalVirtual = 0;
        ullAvailVirtual = 0;
        ullAvailExtendedVirtual = 0;
    }
}

