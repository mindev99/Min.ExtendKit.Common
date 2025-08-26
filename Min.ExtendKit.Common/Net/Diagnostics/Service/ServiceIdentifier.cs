using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics.Service;

/// <summary>
/// 提供 TCP/UDP 常见端口号到服务名称的映射，用于快速识别服务类型。
/// </summary>
/// <remarks>
/// 本类封装了常用端口（HTTP, HTTPS, FTP 等）以及一些偏门和冷门端口（数据库、远程管理、即时通信等）。
/// 提供方法根据端口号快速获取服务名称，同时支持枚举端口类型。
/// </remarks>
public static class ServiceIdentifier
{
    /// <summary>
    /// 内部端口字典，端口号 => 服务名称
    /// </summary>
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, string> Services = [];

    // 静态构造函数，初始化常见服务
    static ServiceIdentifier()
    {
        try
        {
            foreach (var kv in GetDefaultServices())
                Services[kv.Key] = kv.Value;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ServiceIdentifier] 初始化失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 根据端口号返回常见服务名称。
    /// </summary>
    /// <param name="port">TCP/UDP 端口号</param>
    /// <returns>
    /// 返回端口对应的服务名称。如果未找到对应服务，返回 "Unknown"。
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">端口号不在 0~65535 范围内抛出异常</exception>
    public static string GetServiceName(int port)
    {
        //if (port < 0 || port > 65535)
        //    throw new ArgumentOutOfRangeException(nameof(port), "端口号必须在 0~65535 之间。");

        //return Services.TryGetValue(port, out var name) ? name : "Unknown";
        try
        {
            if (port < 0 || port > 65535)
                return "Unknown"; // ✅ 不再抛异常，避免影响扫描

            if (Services != null && Services.TryGetValue(port, out var name))
                return name;

            return "Unknown";
        }
        catch (Exception ex)
        {
            // ✅ 捕获所有异常，保证扫描器不会中断
            Debug.WriteLine($"GetServiceName 异常: {ex.Message}");
            return "Unknown";
        }
    }

    /// <summary>
    /// 获取默认内置的端口服务表
    /// </summary>
    private static Dictionary<int, string> GetDefaultServices()
    {
        return new Dictionary<int, string>
            {
                // 标准常用端口
                { 20, "FTP Data" },
                { 21, "FTP Control" },
                { 22, "SSH" },
                { 23, "Telnet" },
                { 25, "SMTP" },
                { 53, "DNS" },
                { 67, "DHCP Server" },
                { 68, "DHCP Client" },
                { 69, "TFTP" },
                { 80, "HTTP" },
                { 110, "POP3" },
                { 123, "NTP" },
                { 143, "IMAP" },
                { 161, "SNMP" },
                { 162, "SNMP Trap" },
                { 443, "HTTPS" },
                { 445, "Microsoft-DS" },
                { 465, "SMTPS" },
                { 514, "Syslog" },
                { 587, "SMTP Submission" },
                { 636, "LDAPS" },
                { 993, "IMAPS" },
                { 995, "POP3S" },

                // 数据库类
                { 1433, "MSSQL" },
                { 1434, "MSSQL Monitor" },
                { 1521, "Oracle DB" },
                { 3306, "MySQL" },
                { 3389, "RDP" },
                { 5432, "PostgreSQL" },
                { 6379, "Redis" },
                { 27017, "MongoDB" },

                // 运维工具 / 管理
                { 5900, "VNC" },
                { 5985, "WinRM HTTP" },
                { 5986, "WinRM HTTPS" },
                { 8000, "HTTP Alternate" },
                { 8080, "HTTP Proxy" },
                { 8443, "HTTPS Alternate" },

                // 其他常见
                { 2049, "NFS" },
                { 3260, "iSCSI" },
                { 5000, "UPnP / HTTP Alternate" },
                { 5060, "SIP" },
                { 5061, "SIP TLS" },
                { 11211, "Memcached" }
            };
    }

    /// <summary>
    /// 注册或覆盖一个服务名
    /// </summary>
    public static void RegisterService(int port, string name)
    {
        if (port < 0 || port > 65535) return;
        if (string.IsNullOrWhiteSpace(name)) return;
        Services[port] = name;
    }

    /// <summary>
    /// 获取所有已知端口及其服务名称。
    /// </summary>
    /// <returns>返回端口号到服务名称的只读字典副本。</returns>
    public static IReadOnlyDictionary<int, string> GetAllServices()
    {
        return new Dictionary<int, string>(Services);
    }

    /// <summary>
    /// 根据服务名称搜索对应的端口号列表（不区分大小写）。
    /// </summary>
    /// <param name="serviceName">服务名称，如 "HTTP"</param>
    /// <returns>返回匹配的端口号列表，如果未找到返回空列表。</returns>
    /// <exception cref="ArgumentNullException">服务名称为 null 或空时抛出异常</exception>
    public static List<int> GetPortsByService(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentNullException(nameof(serviceName), "服务名称不能为空。");

        return Services
            .Where(kv => kv.Value.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Key)
            .ToList();
    }

    /// <summary>
    /// 判断指定端口是否在已知服务列表中。
    /// </summary>
    /// <param name="port">端口号</param>
    /// <returns>如果端口在已知服务中，返回 true，否则 false。</returns>
    public static bool IsKnownPort(int port)
    {
        return Services.ContainsKey(port);
    }

    /// <summary>
    /// 添加自定义端口与服务名称映射。
    /// </summary>
    /// <param name="port">端口号</param>
    /// <param name="serviceName">服务名称</param>
    /// <exception cref="ArgumentOutOfRangeException">端口号不合法</exception>
    /// <exception cref="ArgumentNullException">服务名称为空</exception>
    public static void AddCustomService(int port, string serviceName)
    {
        if (port < 0 || port > 65535)
            throw new ArgumentOutOfRangeException(nameof(port), "端口号必须在 0~65535 之间。");

        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentNullException(nameof(serviceName), "服务名称不能为空。");

        Services[port] = serviceName;
    }
}
