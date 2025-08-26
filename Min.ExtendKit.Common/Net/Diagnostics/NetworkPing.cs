using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.Text;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 提供 Ping 测试相关的网络诊断功能，包括同步、异步、多参数、统计信息及快速测试方法。
/// </summary>
[SupportedOSPlatform("windows")]
public class NetworkPing
{
    /// <summary>
    /// 对指定主机执行同步 Ping 测试。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="count">Ping 测试次数，默认 4 次。</param>
    /// <param name="timeout">超时时间（毫秒），默认 1000。</param>
    /// <returns>Ping 结果的字符串报告。</returns>
    public static string PingHost(string host, int count = 4, int timeout = 1000)
    {
        StringBuilder result = new();
        using Ping ping = new();

        for (int i = 0; i < count; i++)
        {
            try
            {
                var reply = ping.Send(host, timeout);
                if (reply.Status == IPStatus.Success)
                    result.AppendLine($"Reply from {reply.Address}: time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl}");
                else
                    result.AppendLine($"Request timed out ({reply.Status})");
            }
            catch (Exception ex)
            {
                result.AppendLine($"Ping error: {ex.Message}");
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 对指定主机执行同步 Ping 测试。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="count">Ping 测试次数。</param>
    /// <param name="timeout">超时时间（毫秒）。</param>
    /// <param name="bufferSize">数据包大小（字节）。</param>
    /// <param name="dontFragment">是否禁止分片（默认 false）。</param>
    /// <returns>Ping 汇总结果。</returns>
    public static PingReport PingHost(string host, int count = 4, int timeout = 1000, int bufferSize = 32, bool dontFragment = false)
    {
        var report = new PingReport { Host = host, Count = count };
        using Ping ping = new();

        byte[] buffer = new byte[bufferSize];
        new Random().NextBytes(buffer); // 填充随机数据

        var options = new PingOptions { DontFragment = dontFragment };

        for (int i = 0; i < count; i++)
        {
            try
            {
                PingReply reply = ping.Send(host, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    report.SuccessCount++;
                    report.Latencies.Add(reply.RoundtripTime);
                    report.RawOutputs.Add($"Reply from {reply.Address}: time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl}");
                }
                else
                {
                    report.RawOutputs.Add($"Request timed out ({reply.Status})");
                }
            }
            catch (Exception ex)
            {
                report.RawOutputs.Add($"Ping error: {ex.Message}");
            }
        }

        return report;
    }

    /// <summary>
    /// 对指定主机执行异步 Ping 测试。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="count">Ping 次数。</param>
    /// <param name="timeout">超时时间（毫秒）。</param>
    /// <param name="bufferSize">数据包大小（字节）。</param>
    /// <param name="dontFragment">是否禁止分片。</param>
    /// <returns>Ping 汇总结果。</returns>
    public static async Task<PingReport> PingHostAsync(string host, int count = 4, int timeout = 1000, int bufferSize = 32, bool dontFragment = false)
    {
        var report = new PingReport { Host = host, Count = count };
        using Ping ping = new();

        byte[] buffer = new byte[bufferSize];
        new Random().NextBytes(buffer);

        var options = new PingOptions { DontFragment = dontFragment };

        for (int i = 0; i < count; i++)
        {
            try
            {
                var reply = await ping.SendPingAsync(host, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    report.SuccessCount++;
                    report.Latencies.Add(reply.RoundtripTime);
                    report.RawOutputs.Add($"Reply from {reply.Address}: time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl}");
                }
                else
                {
                    report.RawOutputs.Add($"Request timed out ({reply.Status})");
                }
            }
            catch (Exception ex)
            {
                report.RawOutputs.Add($"Ping error: {ex.Message}");
            }

            await Task.Delay(200); // 模拟命令行 ping 的间隔
        }

        return report;
    }

    /// <summary>
    /// 异步多次 Ping 测试指定 IP 或域名。
    /// </summary>
    /// <param name="host">目标 IP 或域名。</param>
    /// <param name="count">Ping 次数。</param>
    /// <param name="timeout">单次超时时间（毫秒）。</param>
    /// <returns>
    /// 返回一个包含每次 Ping 延迟（毫秒）的列表。失败的返回值为 <c>null</c>。  
    /// 你可以根据结果计算丢包率、最大/最小/平均延迟。
    /// </returns>
    public static async Task<List<long?>> PingAsync(string host, int count = 4, int timeout = 1000)
    {
        var results = new List<long?>();
        using var ping = new Ping();

        for (int i = 0; i < count; i++)
        {
            try
            {
                var reply = await ping.SendPingAsync(host, timeout);
                if (reply.Status == IPStatus.Success)
                    results.Add(reply.RoundtripTime);
                else
                    results.Add(null);
            }
            catch
            {
                results.Add(null);
            }

            // 每次 Ping 之间加一点延迟，模拟命令行 ping 的效果
            await Task.Delay(200);
        }

        return results;
    }

    /// <summary>
    /// 获取多次 Ping 的统计结果（成功次数、丢包率、平均延迟）。
    /// </summary>
    public static (int SuccessCount, int FailCount, double LossRate, double? AvgLatency) GetPingStatistics(List<long?> results)
    {
        int success = results.Count(r => r.HasValue);
        int fail = results.Count - success;
        double lossRate = results.Count > 0 ? (fail * 100.0 / results.Count) : 100.0;
        double? avg = success > 0 ? results.Where(r => r.HasValue).Average(r => r!.Value) : null;

        return (success, fail, lossRate, avg);
    }

    #region 扩展功能

    /// <summary>
    /// 快速 Ping 测试：返回丢包率和平均延迟，仅适合快速检测。
    /// </summary>
    public static async Task<(double LossRate, double? AvgLatency)> QuickPingAsync(string host, int count = 3, int timeout = 500)
    {
        var report = await PingHostAsync(host, count, timeout);
        return (report.LossRate, report.AvgLatency);
    }

    /// <summary>
    /// 检查是否所有 Ping 都成功（可用于判断目标是否完全可达）。
    /// </summary>
    public static bool IsFullyReachable(PingReport report) => report.FailCount == 0;

    /// <summary>
    /// 返回 Ping 延迟的最大值。
    /// </summary>
    public static long? MaxLatency(PingReport report) => report.MaxLatency;

    /// <summary>
    /// 返回 Ping 延迟的最小值。
    /// </summary>
    public static long? MinLatency(PingReport report) => report.MinLatency;

    /// <summary>
    /// 返回 Ping 延迟的平均值。
    /// </summary>
    public static double? AverageLatency(PingReport report) => report.AvgLatency;

    /// <summary>
    /// 批量 Ping 多个目标，可传入自定义主机列表，打印详细报告。
    /// </summary>
    /// <param name="hosts">要 Ping 的主机列表，如果为空或 null，将使用默认常用地址。</param>
    /// <param name="output">输出方法，可用于控制台或 UI（默认为 Console.WriteLine）。</param>
    public static async Task PingMultipleHostsAsync(IEnumerable<string>? hosts = null, Action<string>? output = null)
    {
        // 如果用户没有传入 hosts，使用默认常用地址
        string[] defaultHosts = ["localhost", "127.0.0.1"];
        var testHosts = hosts?.ToArray() ?? defaultHosts;

        foreach (var host in testHosts)
        {
            output?.Invoke($"=== Testing {host} ===");
            var report = await PingHostAsync(host);
            output?.Invoke(report.ToString());
        }
    }

    #endregion

}




