using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Core.Struct;

/// <summary>
/// 包含处理器和系统相关信息的结构体，用于 <see cref="GetSystemInfo"/> 方法。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct SYSTEM_INFO
{
    /// <summary>
    /// 处理器架构。常见值：
    /// 0 = x86, 5 = ARM, 6 = Itanium, 9 = x64, 12 = ARM64
    /// </summary>
    public ushort wProcessorArchitecture;

    /// <summary>
    /// 保留字段，始终为 0。
    /// </summary>
    public ushort wReserved;

    /// <summary>
    /// 系统页大小（字节）。
    /// </summary>
    public uint dwPageSize;

    /// <summary>
    /// 系统可用的最低应用程序地址。
    /// </summary>
    public IntPtr lpMinimumApplicationAddress;

    /// <summary>
    /// 系统可用的最高应用程序地址。
    /// </summary>
    public IntPtr lpMaximumApplicationAddress;

    /// <summary>
    /// 活跃处理器掩码（每个位表示一个逻辑处理器）。
    /// </summary>
    public IntPtr dwActiveProcessorMask;

    /// <summary>
    /// 系统中逻辑处理器的数量。
    /// </summary>
    public uint dwNumberOfProcessors;

    /// <summary>
    /// 处理器类型，已废弃，可忽略。
    /// </summary>
    public uint dwProcessorType;

    /// <summary>
    /// 内存分配粒度（字节）。
    /// </summary>
    public uint dwAllocationGranularity;

    /// <summary>
    /// 处理器等级（与 CPU 代号相关）。
    /// </summary>
    public ushort wProcessorLevel;

    /// <summary>
    /// 处理器修订号。
    /// </summary>
    public ushort wProcessorRevision;
}

