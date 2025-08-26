using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using Min.ExtendKit.Common.Net.Diagnostics.Models;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 提供对目标主机进行详细网络跟踪的功能。
/// 包括每跳 IP、协议、端口、往返时间（RTT）、连接状态等详细信息。
/// 适合用于诊断网络路由、延迟、跳数及异常节点。
/// </summary>
public class DetailedTrace
{
    /// <summary>
    /// 网络跟踪模式
    /// </summary>
    public enum TraceMode { ICMP, TCP }

    /// <summary>
    /// 每跳完成回调事件，可用于 UI 实时刷新。
    /// </summary>
    public event Action<TraceHop>? OnHopCompleted;

    /// <summary>
    /// 异常回调事件，用于捕获内部异常。
    /// </summary>
    public event Action<Exception>? OnError;

    /// <summary>
    /// 目标主机名或 IP 地址。
    /// </summary>
    public string Host { get; private set; }

    /// <summary>
    /// 目标端口号，通常为 80（HTTP）或 443（HTTPS）。
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// 最大跳数（TTL），默认值为 30。
    /// </summary>
    public int MaxHops { get; private set; } = 30;

    /// <summary>
    /// 每跳尝试次数
    /// </summary>
    public int RetryPerHop { get; private set; } = 3;

    /// <summary>
    /// 每跳超时时间（毫秒），默认值为 3000。
    /// </summary>
    public int Timeout { get; private set; } = 3000;

    /// <summary>
    /// 记录每个跳点的详细信息列表。
    /// 每个元素为 <see cref="TraceHop"/>，包含 IP、RTT、状态等信息。
    /// </summary>
    public List<TraceHop> Hops { get; private set; } = new List<TraceHop>();

    /// <summary>
    /// 创建 <see cref="DetailedTrace"/> 实例。
    /// </summary>
    /// <param name="host">目标主机名或 IP 地址。</param>
    /// <param name="port">目标端口号，默认 80。</param>
    /// <param name="maxHops">最大跳数（TTL），默认 30。</param>
    /// <param name="timeout">每跳超时时间（毫秒），默认 3000。</param>
    /// <param name="retryPerHop">每跳尝试次数，默认 3。</param>
    public DetailedTrace(string host, int port = 80, int maxHops = 30, int timeout = 3000, int retryPerHop = 3)
    {
        Host = host;
        Port = port;
        MaxHops = maxHops;
        Timeout = timeout;
        RetryPerHop = retryPerHop;
    }

    /// <summary>
    /// 执行 ICMP 跟踪，每跳尝试多次，并可触发回调。
    /// 支持 IPv4 和 IPv6。
    /// </summary>
    private async Task TraceICMPAsync()
    {
        Hops.Clear();
        IPAddress[] addresses;

        try { addresses = await Dns.GetHostAddressesAsync(Host); }
        catch (Exception ex)
        {
            Hops.Add(new TraceHop { Hop = 0, IP = Host, Status = "DNS Fail" });
            OnHopCompleted?.Invoke(Hops[0]);
            OnError?.Invoke(ex);
            return;
        }

        foreach (var ip in addresses)
        {
            for (int ttl = 1; ttl <= MaxHops; ttl++)
            {
                TraceHop hop = new() { Hop = ttl, IP = ip.ToString() };
                long totalRtt = 0;
                int successCount = 0;

                for (int attempt = 0; attempt < RetryPerHop; attempt++)
                {
                    try
                    {
                        using var ping = new Ping();
                        PingOptions options = new(ttl, true);
                        var reply = await ping.SendPingAsync(ip, Timeout, new byte[32], options);

                        if (reply.Status == IPStatus.Success || reply.Status == IPStatus.TtlExpired)
                        {
                            totalRtt += reply.RoundtripTime;
                            successCount++;
                            hop.IP = reply.Address?.ToString() ?? ip.ToString();
                        }

                        if (reply.Status == IPStatus.Success)
                        {
                            hop.Status = "Success";
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        hop.Status = "Error: " + ex.Message;
                        OnError?.Invoke(ex);
                    }
                }

                hop.RTT = successCount > 0 ? totalRtt / successCount : 0;

                if (hop.Status != "Success")
                    hop.Status = successCount > 0 ? "TtlExpired" : "Timeout";

                // 尝试反向解析 Hostname
                try
                {
                    var entry = await Dns.GetHostEntryAsync(hop.IP);
                    hop.Hostname = entry.HostName;
                }
                catch { hop.Hostname = string.Empty; }

                Hops.Add(hop);
                OnHopCompleted?.Invoke(hop);

                if (hop.Status == "Success")
                    return;
            }
        }
    }

    /// <summary>
    /// 执行 TCP 跟踪，仅记录目标主机端口连通性。
    /// </summary>
    private async Task TraceTCPAsync()
    {
        Hops.Clear();
        IPAddress[] addresses;

        try { addresses = await Dns.GetHostAddressesAsync(Host); }
        catch (Exception ex)
        {
            var hop = new TraceHop { Hop = 0, IP = Host, Status = "DNS Fail" };
            Hops.Add(hop);
            OnHopCompleted?.Invoke(hop);
            OnError?.Invoke(ex);
            return;
        }

        foreach (var ip in addresses)
        {
            var sw = Stopwatch.StartNew();
            var hop = new TraceHop { Hop = 1, IP = ip.ToString() };

            try
            {
                using var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = Timeout,
                    SendTimeout = Timeout,
                    LingerState = new LingerOption(true, 0)
                };

                var connectTask = socket.ConnectAsync(ip, Port);
                if (await Task.WhenAny(connectTask, Task.Delay(Timeout)) == connectTask)
                    hop.Status = "Connected";
                else
                    hop.Status = "Timeout";

                hop.RTT = sw.ElapsedMilliseconds;
            }
            catch (SocketException ex)
            {
                hop.Status = ex.SocketErrorCode.ToString();
                hop.RTT = sw.ElapsedMilliseconds;
                OnError?.Invoke(ex);
            }

            Hops.Add(hop);
            OnHopCompleted?.Invoke(hop);
        }
    }

    /// <summary>
    /// 异步执行详细网络跟踪。
    /// </summary>
    /// <returns>返回一个 <see cref="Task"/>，完成后 Hops 列表包含每跳的详细信息。</returns>
    /// <remarks>
    /// 每跳会尝试通过指定端口建立 TCP 连接或发送 ICMP 包。
    /// 跟踪完成后，可通过 <see cref="Hops"/> 查看每跳 IP、RTT、状态。
    /// </remarks>
    public async Task TraceAsync(TraceMode mode = TraceMode.ICMP)
    {
        Hops.Clear();
        try
        {
            if (mode == TraceMode.ICMP)
            {
                await TraceICMPAsync();
            }
            else
            {
                await TraceTCPAsync();
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }

    /// <summary>
    /// 将 Hops 列表保存为 CSV 文件，方便 Excel 或其他工具打开查看。
    /// </summary>
    /// <param name="path">CSV 文件保存路径（含文件名，如 "trace.csv"）</param>
    /// <remarks>
    /// CSV 格式包含列：Hop, IP, Hostname, Status, RTT(ms)
    /// 会覆盖已存在文件。
    /// </remarks>
    public void SaveReportCsv(string path)
    {
        var sb = new System.Text.StringBuilder();

        // 写入表头
        sb.AppendLine("Hop,IP,Hostname,Status,RTT(ms)");

        // 写入每个跳点信息
        foreach (var hop in Hops)
        {
            // IP 或 Hostname 里可能包含逗号或引号，做简单转义
            string ip = hop.IP.Replace("\"", "\"\"");
            string hostname = hop.Hostname.Replace("\"", "\"\"");
            string status = hop.Status.Replace("\"", "\"\"");

            sb.AppendLine($"{hop.Hop},\"{ip}\",\"{hostname}\",\"{status}\",{hop.RTT}");
        }

        File.WriteAllText(path, sb.ToString(), System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// 将 Hops 列表转换为可读文本报告。
    /// </summary>
    /// <returns>字符串形式的详细网络跟踪报告。</returns>
    /// <example>
    /// <code>
    /// var trace = new DetailedNetworkTrace("www.google.com");
    /// await trace.TraceAsync();
    /// Console.WriteLine(trace.ToReport());
    /// </code>
    /// </example>
    public string ToReport()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"--- Detailed Network Trace Report for {Host}:{Port} ---");
        foreach (var hop in Hops)
        {
            sb.AppendLine(hop.ToString());
        }
        return sb.ToString();
    }

    /// <summary>
    /// 检查是否成功到达目标主机（最后一跳状态为 Connected）。
    /// </summary>
    /// <returns>成功返回 true，否则 false。</returns>
    public bool IsReachable()
    {
        if (Hops.Count == 0) return false;
        var last = Hops[^1];
        return last.Status == "Success" || last.Status == "Connected";
    }

    /// <summary>
    /// 详细网络跟踪
    /// </summary>
    /// <param name="host">目标域名或IP</param>
    /// <param name="port">端口（HTTP 80, HTTPS 443）</param>
    /// <param name="maxHops">最大跳数</param>
    /// <param name="timeout">每跳超时时间（ms）</param>
    /// <returns>每跳信息列表 + 最终目标状态</returns>
    [Obsolete("不在使用静态方法，请使用实例对象！")]
    public static async Task<List<TraceHop>> TraceAsync(string host, int port = 443, int maxHops = 30, int timeout = 3000)
    {
        List<TraceHop> hops = [];
        IPAddress[] addresses;
        try
        {
            addresses = await Dns.GetHostAddressesAsync(host);
        }
        catch
        {
            hops.Add(new TraceHop { Hop = 0, IP = host, Status = "DNS Fail" });
            return hops;
        }

        var targetIP = addresses[0];

        for (int ttl = 1; ttl <= maxHops; ttl++)
        {
            TraceHop hop = new() { Hop = ttl };
            var sw = Stopwatch.StartNew();
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.ReceiveTimeout = timeout;
                socket.SendTimeout = timeout;
                socket.LingerState = new LingerOption(true, 0);

                try
                {
                    // 设置 TTL
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, ttl);

                    // 异步连接目标
                    var task = socket.ConnectAsync(targetIP, port);
                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        hop.RTT = sw.ElapsedMilliseconds;
                        hop.IP = targetIP.ToString();
                        hop.Status = "Connected";
                        hops.Add(hop);
                        break; // 目标到达
                    }
                    else
                    {
                        hop.Status = "Timeout";
                        hop.IP = targetIP.ToString();
                    }
                }
                catch (SocketException ex)
                {
                    hop.Status = ex.SocketErrorCode.ToString();
                    hop.IP = targetIP.ToString();
                }
            }
            hops.Add(hop);
        }

        return hops;
    }

    /// <summary>
    /// 快速打印 TraceHop 信息
    /// </summary>
    [Obsolete("不在使用静态方法，请使用实例对象！")]
    public static void PrintReport(IEnumerable<TraceHop> hops)
    {
        foreach (var hop in hops)
        {
            Debug.WriteLine(hop.ToString());
            Console.WriteLine(hop.ToString());
            Debug.WriteLine(hop.ToString());
        }
    }

}



