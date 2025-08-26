using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics.Models;

/// <summary>
/// 表示 Traceroute 或详细网络跟踪中的单个跳点信息。
/// </summary>
public class TraceHop
{
    /// <summary>
    /// 反向解析得到的主机名，如果未解析成功则为空字符串。
    /// </summary>
    public string Hostname { get; set; } = string.Empty;

    /// <summary>
    /// 跳点编号（TTL 值），从 1 开始。
    /// </summary>
    public int Hop { get; set; }

    /// <summary>
    /// 此跳点的 跳点 IP 地址（IPv4 或 IPv6）。如果无法解析或超时，默认为 "*"。
    /// </summary>
    public string IP { get; set; } = "*";

    /// <summary>
    /// 此跳点的往返时间 (Round Trip Time, RTT)，单位为毫秒。
    /// 如果无法测量 RTT，则为 null。
    /// </summary>
    public long? RTT { get; set; }

    /// <summary>
    /// 此跳点的状态信息。
    /// 常见值包括：
    /// <list type="bullet">
    /// <item><description>"Connected" 表示成功到达目标端口。</description></item>
    /// <item><description>"Timeout" 表示请求超时。</description></item>
    /// <item><description>SocketErrorCode 的枚举值，例如 "NetworkUnreachable"、"ConnectionRefused"。</description></item>
    /// <item><description>"Unknown" 表示状态未定义或异常。</description></item>
    /// </list>
    /// </summary>
    public string Status { get; set; } = "Unknown";

    /// <summary>
    /// 返回此跳点的可读字符串表示，方便调试和日志输出。
    /// 格式示例：
    /// <code>
    /// Hop 1: 192.168.1.1 (Connected) RTT=5ms
    /// Hop 2: * (Timeout) RTT=*
    /// </code>
    /// </summary>
    /// <returns>包含跳点编号、IP、状态和 RTT 的字符串。</returns>
    public override string ToString() => $"Hop {Hop}: {IP} ({Status}) RTT={RTT?.ToString() ?? "*"}ms";
}
