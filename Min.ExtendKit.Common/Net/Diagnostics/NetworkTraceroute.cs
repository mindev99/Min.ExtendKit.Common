using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.Text;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 提供 Traceroute 路由跟踪功能，包括同步/异步、统计信息、快速诊断等。
/// </summary>
[SupportedOSPlatform("windows")]
public static class NetworkTraceroute
{
    /// <summary>
    /// 执行简单的路由跟踪。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="maxHops">最大跳数，默认 30。</param>
    /// <param name="timeout">每跳超时时间（毫秒），默认 3000。</param>
    /// <returns>返回路由跟踪字符串报告。</returns>
    public static string TracerouteSimple(string host, int maxHops = 30, int timeout = 3000)
    {
        StringBuilder result = new();
        using Ping ping = new();
        PingOptions options = new(1, true);

        for (int ttl = 1; ttl <= maxHops; ttl++)
        {
            options.Ttl = ttl;
            try
            {
                var reply = ping.Send(host, timeout, new byte[32], options);
                string address = reply.Address?.ToString() ?? "*";
                result.AppendLine($"Hop {ttl}: {reply.Status} {address}");
                if (reply.Status == IPStatus.Success) break;
            }
            catch (Exception ex)
            {
                result.AppendLine($"Hop {ttl}: Error {ex.Message}");
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 执行简单的同步 Traceroute，并返回可读文本报告。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="maxHops">最大跳数，默认值为 30。</param>
    /// <param name="timeout">每跳的超时时间（毫秒），默认值为 3000。</param>
    /// <returns>返回 Traceroute 文本报告，包含每跳信息。</returns>
    public static string Traceroute(string host, int maxHops = 30, int timeout = 3000)
    {
        var hops = TracerouteHops(host, maxHops, timeout);
        StringBuilder sb = new();
        foreach (var hop in hops)
            sb.AppendLine(hop.ToString());
        return sb.ToString();
    }

    /// <summary>
    /// 执行同步 Traceroute，并返回每跳详细信息列表。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="maxHops">最大跳数，默认 30。</param>
    /// <param name="timeout">每跳超时时间（毫秒），默认 3000。</param>
    /// <returns>返回每跳信息的列表，每个元素为 <see cref="HopInfo"/>。</returns>
    public static List<HopInfo> TracerouteHops(string host, int maxHops = 30, int timeout = 3000)
    {
        List<HopInfo> hops = [];
        using Ping ping = new();
        PingOptions options = new(1, true);
        byte[] buffer = new byte[32];

        for (int ttl = 1; ttl <= maxHops; ttl++)
        {
            options.Ttl = ttl;
            var hop = new HopInfo { Hop = ttl };
            try
            {
                var reply = ping.Send(host, timeout, buffer, options);
                hop.Status = reply.Status;
                hop.Address = reply.Address?.ToString() ?? "*";
                hop.RoundtripTime = reply.Status == IPStatus.Success ? reply.RoundtripTime : null;
                hops.Add(hop);

                if (reply.Status == IPStatus.Success)
                    break;
            }
            catch
            {
                hop.Status = IPStatus.Unknown;
                hop.Address = "*";
                hop.RoundtripTime = null;
                hops.Add(hop);
            }
        }

        return hops;
    }

    /// <summary>
    /// 异步执行 Traceroute，并返回每跳详细信息列表。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="maxHops">最大跳数，默认 30。</param>
    /// <param name="timeout">每跳超时时间（毫秒），默认 3000。</param>
    /// <returns>每跳信息列表，每个元素为 <see cref="HopInfo"/>。</returns>
    public static async Task<List<HopInfo>> TracerouteAsync(string host, int maxHops = 30, int timeout = 3000)
    {
        List<HopInfo> hops = [];
        using Ping ping = new();
        PingOptions options = new(1, true);
        byte[] buffer = new byte[32];

        for (int ttl = 1; ttl <= maxHops; ttl++)
        {
            options.Ttl = ttl;
            var hop = new HopInfo { Hop = ttl };
            try
            {
                var reply = await ping.SendPingAsync(host, timeout, buffer, options);
                hop.Status = reply.Status;
                hop.Address = reply.Address?.ToString() ?? "*";
                hop.RoundtripTime = reply.Status == IPStatus.Success ? reply.RoundtripTime : null;
                hops.Add(hop);

                if (reply.Status == IPStatus.Success)
                    break;
            }
            catch
            {
                hop.Status = IPStatus.Unknown;
                hop.Address = "*";
                hop.RoundtripTime = null;
                hops.Add(hop);
            }

            await Task.Delay(100); // 每跳间隔
        }

        return hops;
    }

    #region 扩展功能 

    /// <summary>
    /// 将 Traceroute 结果转换为可读文本报告。
    /// </summary>
    public static string ToReport(IEnumerable<HopInfo> hops)
    {
        StringBuilder sb = new();
        sb.AppendLine("--- Traceroute Report ---");
        foreach (var hop in hops)
        {
            sb.AppendLine(hop.ToString());
        }
        return sb.ToString();
    }

    /// <summary>
    /// 判断目标是否可达（最后一跳成功）。
    /// </summary>
    /// <param name="hops">Traceroute 每跳信息列表。</param>
    /// <returns>若最后一跳成功返回 true，否则 false。</returns>
    public static bool IsReachable(List<HopInfo> hops) => hops.Count > 0 && hops.Last().Status == IPStatus.Success;

    /// <summary>
    /// 获取最大 RTT（往返延迟）。
    /// </summary>
    /// <param name="hops">Traceroute 每跳信息列表。</param>
    /// <returns>最大 RTT（毫秒），若所有跳失败则返回 null。</returns>
    public static long? MaxRTT(List<HopInfo> hops) => hops.Where(h => h.RoundtripTime.HasValue).Max(h => h.RoundtripTime);

    /// <summary>
    /// 获取最小 RTT（往返延迟）。
    /// </summary>
    /// <param name="hops">Traceroute 每跳信息列表。</param>
    /// <returns>最小 RTT（毫秒），若所有跳失败则返回 null。</returns>
    public static long? MinRTT(List<HopInfo> hops) => hops.Where(h => h.RoundtripTime.HasValue).Min(h => h.RoundtripTime);

    /// <summary>
    /// 获取平均 RTT（往返延迟）。
    /// </summary>
    /// <param name="hops">Traceroute 每跳信息列表。</param>
    /// <returns>平均 RTT（毫秒），若所有跳失败则返回 null。</returns>
    public static double? AvgRTT(IEnumerable<HopInfo> hops)
    {
        var list = hops.Where(h => h.RoundtripTime.HasValue).Select(h => h.RoundtripTime!.Value).ToList();
        return list.Count > 0 ? list.Average() : null;
    }
    /// <summary>
    /// 检测 Traceroute 是否有节点异常（丢包或 "*" 节点）。
    /// </summary>
    public static List<int> DetectAnomalies(IEnumerable<HopInfo> hops)
    {
        List<int> anomalies = [];
        double? avg = AvgRTT(hops);
        int index = 1;
        foreach (var hop in hops)
        {
            if (!hop.RoundtripTime.HasValue || (avg.HasValue && hop.RoundtripTime > avg * 2))
                anomalies.Add(index);
            index++;
        }
        return anomalies;
    }

    /// <summary>
    /// 快速 Traceroute 测试，仅返回平均 RTT 和最大 RTT。
    /// </summary>
    public static async Task<(double? Avg, long? Max)> QuickTracerouteAsync(string host, int maxHops = 10)
    {
        var hops = await TracerouteAsync(host, maxHops);
        return (AvgRTT(hops), MaxRTT(hops));
    }

    /// <summary>
    /// 判断 Traceroute 路径是否经过私有 IP。
    /// </summary>
    /// <param name="hops">Traceroute 每跳信息列表。</param>
    /// <returns>如果路径中包含私有 IP 返回 true，否则 false。</returns>
    public static bool PassedPrivateIP(List<HopInfo> hops) => hops.Any(h => h.IsPrivateIP);

    /// <summary>
    /// 将 Traceroute 结果导出为 CSV 文件。
    /// </summary>
    public static void ExportToCsv(string filePath, IEnumerable<HopInfo> hops)
    {
        var lines = new List<string> { "Hop,Address,Status,RTT(ms)" };
        lines.AddRange(hops.Select(h => $"{h.Hop},{h.Address},{h.Status},{h.RoundtripTime ?? -1}"));
        File.WriteAllLines(filePath, lines);
    }

    /// <summary>
    /// 在 Debug 窗口输出 Traceroute 信息（用于调试）。
    /// </summary>
    /// <param name="hops">Traceroute 每跳信息列表。</param>
    public static void DebugOutput(List<HopInfo> hops)
    {
        foreach (var hop in hops)
        {
            System.Diagnostics.Debug.WriteLine(hop.ToString());
        }
    }

    /// <summary>
    /// 批量 Traceroute 测试多个主机。
    /// </summary>
    public static async Task BatchTracerouteAsync(IEnumerable<string> hosts, Action<string>? output = null)
    {
        output ??= s => Debug.WriteLine(s);
        foreach (var host in hosts)
        {
            output($"=== Traceroute {host} ===");
            var hops = await TracerouteAsync(host);
            output(ToReport(hops));
        }
    }

    /// <summary>
    /// 异步批量 Traceroute 多个目标。
    /// </summary>
    /// <param name="hosts">目标主机名或 IP 列表。</param>
    /// <param name="maxHops">最大跳数，默认 30。</param>
    /// <param name="timeout">每跳超时时间（毫秒），默认 3000。</param>
    /// <returns>返回字典，键为目标主机，值为每跳信息列表。</returns>
    public static async Task<Dictionary<string, List<HopInfo>>> TracerouteMultipleAsync(IEnumerable<string> hosts, int maxHops = 30, int timeout = 3000)
    {
        var dict = new Dictionary<string, List<HopInfo>>();
        foreach (var host in hosts)
        {
            dict[host] = await TracerouteAsync(host, maxHops, timeout);
        }
        return dict;
    }

    #endregion

}
