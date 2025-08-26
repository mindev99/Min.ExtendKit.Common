using System.Net.NetworkInformation;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 表示 Traceroute 每跳信息。
/// </summary>
public class HopInfo
{
    /// <summary>跳数（TTL）。</summary>
    public int Hop { get; set; }

    /// <summary>节点 IP 地址。</summary>
    public string Address { get; set; } = "*";

    /// <summary>返回状态。</summary>
    public IPStatus Status { get; set; }

    /// <summary>延迟（毫秒），若失败则为 null。</summary>
    public long? RoundtripTime { get; set; }

    /// <summary>是否为私有 IP 地址。</summary>
    public bool IsPrivateIP => Address.StartsWith("10.") || Address.StartsWith("192.168.") || Address.StartsWith("172.") || Address.StartsWith("*");

    /// <summary>转换为文本描述。</summary>
    public override string ToString()
    {
        string time = RoundtripTime.HasValue ? $"{RoundtripTime}ms" : "*";
        return $"Hop {Hop}: {Address} ({Status}) RTT={time}";
    }
}
