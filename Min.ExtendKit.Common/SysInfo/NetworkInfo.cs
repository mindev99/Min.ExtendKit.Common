using System.Net.NetworkInformation;

namespace Min.ExtendKit.Common.SysInfo;

/// <summary>
/// 提供网络适配器（网卡）相关操作封装。
/// </summary>
public static class NetworkInfo
{
    /// <summary>
    /// 当前活动网卡列表
    /// </summary>
    public static NetworkInterface[] ActiveNetworkInterfaces => NetworkInterface.GetAllNetworkInterfaces();

    /// <summary>
    /// 获取主网卡的 MAC 地址
    /// </summary>
    public static string GetMainMacAddress()
    {
        return System.Net.NetworkInformation.NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// 获取所有启用的网络适配器。
    /// </summary>
    public static IEnumerable<NetworkInterface> GetAllEnabledAdapters()
    {
        return NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up);
    }

    /// <summary>
    /// 获取所有启用网卡的名称列表。
    /// </summary>
    public static IEnumerable<string> GetAdapterNames()
    {
        return GetAllEnabledAdapters().Select(nic => nic.Name);
    }

    /// <summary>
    /// 获取所有启用网卡的描述信息。
    /// </summary>
    public static IEnumerable<string> GetAdapterDescriptions()
    {
        return GetAllEnabledAdapters().Select(nic => nic.Description);
    }

    /// <summary>
    /// 获取指定网卡的 MAC 地址。
    /// </summary>
    /// <param name="adapterName">网卡名称</param>
    /// <returns>MAC 地址字符串，格式为 XX-XX-XX-XX-XX-XX，如果未找到返回 null。</returns>
    public static string? GetMacAddress(string adapterName)
    {
        var nic = GetAllEnabledAdapters().FirstOrDefault(n => n.Name == adapterName);
        return nic?.GetPhysicalAddress().ToString();
    }

    /// <summary>
    /// 获取指定网卡的 IPv4 地址。
    /// </summary>
    /// <param name="adapterName">网卡名称</param>
    /// <returns>IPv4 地址字符串，如果未找到返回 null。</returns>
    public static string? GetIPv4Address(string adapterName)
    {
        var nic = GetAllEnabledAdapters().FirstOrDefault(n => n.Name == adapterName);
        if (nic == null) return null;

        var ipProps = nic.GetIPProperties();
        var ipv4 = ipProps.UnicastAddresses
                          .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        return ipv4?.Address.ToString();
    }

    /// <summary>
    /// 获取指定网卡的 IPv6 地址。
    /// </summary>
    /// <param name="adapterName">网卡名称</param>
    /// <returns>IPv6 地址字符串，如果未找到返回 null。</returns>
    public static string? GetIPv6Address(string adapterName)
    {
        var nic = GetAllEnabledAdapters().FirstOrDefault(n => n.Name == adapterName);
        if (nic == null) return null;

        var ipProps = nic.GetIPProperties();
        var ipv6 = ipProps.UnicastAddresses
                          .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
        return ipv6?.Address.ToString();
    }

    /// <summary>
    /// 获取所有网卡及其 IPv4、MAC 信息的调试输出字符串。
    /// </summary>
    public static string GetDebugInfo()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== 网络适配器信息 ===");
        foreach (var nic in GetAllEnabledAdapters())
        {
            sb.AppendLine($"测试：{GetMacAddress(nic.Name)}\n");
            sb.AppendLine($"名称: {nic.Name}");
            sb.AppendLine($"描述: {nic.Description}");
            sb.AppendLine($"MAC: {nic.GetPhysicalAddress()}");
            var ipv4 = nic.GetIPProperties().UnicastAddresses
                         .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address;
            sb.AppendLine($"IPv4: {ipv4}");
            sb.AppendLine($"IPv4 fun: {GetIPv4Address(nic.Name)}");
            sb.AppendLine($"IPv6 fun: {GetIPv6Address(nic.Name)}");
            sb.AppendLine($"状态: {nic.OperationalStatus}");
            sb.AppendLine("-------------------------");
        }
        return sb.ToString();
    }

}
