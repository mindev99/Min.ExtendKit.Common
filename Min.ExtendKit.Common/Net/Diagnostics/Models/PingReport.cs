using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 表示一次 Ping 测试的汇总结果。
/// </summary>
public class PingReport
{
    /// <summary>目标主机名或 IP。</summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>发送的包数。</summary>
    public int Count { get; set; }

    /// <summary>成功的包数。</summary>
    public int SuccessCount { get; set; }

    /// <summary>失败的包数。</summary>
    public int FailCount => Count - SuccessCount;

    /// <summary>丢包率（百分比）。</summary>
    public double LossRate => Count > 0 ? (FailCount * 100.0 / Count) : 100.0;

    /// <summary>平均延迟（毫秒）。</summary>
    public double? AvgLatency => Latencies.Count > 0 ? Latencies.Average() : null;

    /// <summary>最小延迟（毫秒）。</summary>
    public long? MinLatency => Latencies.Count > 0 ? Latencies.Min() : null;

    /// <summary>最大延迟（毫秒）。</summary>
    public long? MaxLatency => Latencies.Count > 0 ? Latencies.Max() : null;

    /// <summary>所有成功 Ping 的延迟结果。</summary>
    public List<long> Latencies { get; } = new();

    /// <summary>每次 Ping 的原始字符串输出。</summary>
    public List<string> RawOutputs { get; } = new();

    /// <summary>转换为文本报告。</summary>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Ping {Host} with {Count} packets:");
        foreach (var line in RawOutputs)
            sb.AppendLine(line);
        sb.AppendLine($"--- {Host} ping statistics ---");
        sb.AppendLine($"Sent = {Count}, Received = {SuccessCount}, Lost = {FailCount} ({LossRate:F1}% loss)");
        if (Latencies.Count > 0)
            sb.AppendLine($"Round-trip times: Min = {MinLatency}ms, Max = {MaxLatency}ms, Avg = {AvgLatency:F1}ms");
        return sb.ToString();
    }
}

