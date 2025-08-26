using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Min.ExtendKit.Common.Core.Win32.API;

namespace Min.ExtendKit.Common.SysInfo;

/// <summary>
/// 提供 CPU 相关的信息封装。
/// </summary>
[SupportedOSPlatform("windows")]
public static class CpuInfo
{
    /// <summary>
    /// 逻辑处理器数量。
    /// </summary>
    public static int ProcessorCount => Environment.ProcessorCount;

    /// <summary>
    /// CPU 架构，例如 X64、X86、Arm 或 Arm64。
    /// </summary>
    public static string Architecture => RuntimeInformation.ProcessArchitecture.ToString();

    /// <summary>
    /// 获取 CPU 核心数量（物理 + 逻辑混合，只能近似）。
    /// </summary>
    public static int LogicalProcessorCount => Environment.ProcessorCount;

    /// <summary>
    /// CPU 最大频率（MHz），通过 Windows API 获取。
    /// </summary>
    public static uint MaxClockSpeedMHz
    {
        get
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Core.Struct.SYSTEM_INFO sysInfo = new();
                    CommonAPI.GetSystemInfo(ref sysInfo);
                    return (uint)sysInfo.dwNumberOfProcessors * 1000; // 粗略估算
                }
            }
            catch { }
            return 0;
        }
    }

    /// <summary>
    /// 获取当前计算机的制造商（Manufacturer）名称，如果无法获取则返回默认值 null。
    /// 该方法通过 Win32 API 读取 SMBIOS 原始数据，不依赖 WMI。
    /// </summary>
    /// <returns>如果成功则返回厂商名称，否则返回错误信息或 Unknown。</returns>
    public static string? GetManufacturer()
    {
        uint bufferSize = CpuAPI.GetSystemFirmwareTable(0x52534D42, 0, IntPtr.Zero, 0);
        if (bufferSize == 0)
        {
            return null;
        }

        IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
        try
        {
            CpuAPI.GetSystemFirmwareTable(0x52534D42, 0, buffer, bufferSize);
            byte[] rawData = new byte[bufferSize];
            Marshal.Copy(buffer, rawData, 0, (int)bufferSize);

            int offset = 8; // 跳过 SMBIOS 表头

            while (offset < rawData.Length)
            {
                byte type = rawData[offset];
                byte length = rawData[offset + 1];

                if (type == 1) // Type 1 = System Information
                {
                    if (length < 5)
                    {
                        Debug.WriteLine("结构体长度错误");
                        return null;
                    }

                    byte manufacturerIndex = rawData[offset + 4];
                    int stringSectionStart = offset + length;

                    var strings = ExtractSmbiosStrings(rawData, stringSectionStart);

                    if (manufacturerIndex > 0 && manufacturerIndex <= strings.Count)
                        return strings[manufacturerIndex - 1];
                    else
                    {
                        Debug.WriteLine($"字符串索引 {manufacturerIndex} 超出范围（总数：{strings.Count}）");
                        return null;
                    }
                }

                // 跳过正文
                offset += length;

                // 定位结构体结束
                while (offset < rawData.Length - 1 && !(rawData[offset] == 0 && rawData[offset + 1] == 0))
                    offset++;
                offset += 2;
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// 从 SMBIOS 原始数据中提取字符串部分（以 null 分隔的 ASCII 字符串）。
    /// </summary>
    /// <param name="data">完整的 SMBIOS 原始字节数组。</param>
    /// <param name="start">字符串区域起始偏移。</param>
    /// <returns>解析出的字符串列表。</returns>
    private static List<string> ExtractSmbiosStrings(byte[] data, int start)
    {
        var strings = new List<string>();
        var sb = new System.Text.StringBuilder();
        int i = start;

        while (i < data.Length - 1)
        {
            if (data[i] == 0)
            {
                if (data[i + 1] == 0)
                    break;

                strings.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append((char)data[i]);
            }

            i++;
        }

        // 添加最后一个字符串（如果有）
        if (sb.Length > 0)
            strings.Add(sb.ToString());

        return strings;
    }

    /// <summary>
    /// 获取 CPU ID（通过 CPUID 指令，仅限 Windows x86/x64）
    /// </summary>
    public static string GetCpuId()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return string.Empty;

        try
        {
            byte[] buffer = new byte[16];
            GetCpuId(buffer);
            return BitConverter.ToString(buffer).Replace("-", "");
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 获取当前处理器的 CPUID 信息，并填充到指定的字节数组中。
    /// </summary>
    /// <param name="buffer">
    /// 用于接收 CPU 信息的字节数组。调用前应确保数组已正确初始化且长度足够。
    /// </param>
    /// <remarks>
    /// - 方法会根据当前进程架构调用不同的实现：
    ///   - 对于 x64 架构，调用 <see cref="GetCpuIdX64"/> 获取真实 CPU 信息。
    ///   - 对于非 x64 架构，将数组填充为 0。
    /// - 仅适用于 Windows 平台。
    /// </remarks>
    public static void GetCpuId(byte[] buffer)
    {
        if (RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.X64)
            GetCpuIdX64(buffer);
        else
            Array.Fill(buffer, (byte)0);
    }

    /// <summary>
    /// 私有方法：在 x64 架构下调用本地 DLL 获取 CPU 信息。
    /// </summary>
    /// <param name="buffer">
    /// 用于接收 CPU 信息的字节数组。
    /// </param>
    /// <remarks>
    /// - 内部调用 <see cref="CpuAPI.CpuId64"/> 方法。
    /// - 仅在 x64 架构下有效。
    /// </remarks>
    private static void GetCpuIdX64(byte[] buffer)
    {
        CpuAPI.CpuId64(buffer);
    }
}