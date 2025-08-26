using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// UDP 端口扫描器，继承自 PortScannerBase
/// 提供单端口扫描、端口范围扫描、端口列表扫描以及扫描结果导出功能。
/// </summary>
public class UdpPortScanner : PortScannerBase
{
    /// <summary>并发扫描最大线程数</summary>
    public int MaxConcurrency { get; set; } = 50;

    /// <summary>构造函数</summary>
    /// <param name="host">目标主机</param>
    public UdpPortScanner(string host) : base(host) { }

    /// <summary>
    /// 扫描单个 UDP 端口（异步）
    /// </summary>
    /// <param name="port">端口号</param>
    /// <returns>端口扫描结果</returns>
    public override async Task<PortScanResult> ScanPortAsync(int port)
    {
        return await ScanPortAsync(port, null);
    }

    /// <summary>
    /// 扫描单个 UDP 端口（异步）
    /// </summary>
    /// <param name="port">端口号</param>
    /// <param name="payload">可选自定义发送数据</param>
    /// <returns>端口扫描结果</returns>
    public async Task<PortScanResult> ScanPortAsync(int port, byte[]? payload = null)
    {
        var result = new PortScanResult { Port = port };
        var sw = Stopwatch.StartNew();

        try
        {
            using var udp = new UdpClient();
            udp.Client.ReceiveTimeout = Timeout;
            var data = payload ?? [];

            await udp.SendAsync(data, data.Length, Host, port);

            // 简单尝试接收数据，如果超时说明端口可能开放
            var receiveTask = udp.ReceiveAsync();
            //if (await Task.WhenAny(receiveTask, Task.Delay(Timeout)) == receiveTask)
            //    result.Status = PortStatus.Closed; // 收到响应一般说明端口关闭
            //else
            //    result.Status = PortStatus.Open;

            if (await Task.WhenAny(receiveTask, Task.Delay(Timeout)) == receiveTask)
            {
                // 收到回应 → 开放
                result.Status = PortStatus.Open;
            }
            else
            {
                // 超时未回应 → UDP 特有：开放但被过滤
                result.Status = PortStatus.Filtered;
            }


            result.Service = Service.ServiceIdentifier.GetServiceName(port);
        }

        catch (SocketException sex)
        {
            // ❌ 明确的网络异常（比如 ICMP 端口不可达） → 认为是关闭
            result.Status = PortStatus.Closed;
            result.ErrorMessage = sex.Message;
        }
        catch (Exception ex)
        {
            result.Status = PortStatus.Error;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        }

        return result;
    }

    /// <summary>
    /// 批量扫描端口范围（异步，可并发）
    /// </summary>
    /// <param name="startPort">起始端口</param>
    /// <param name="endPort">结束端口</param>
    /// <param name="payload">可选自定义发送数据</param>
    /// <returns>端口扫描结果列表</returns>
    public async Task<List<PortScanResult>> ScanRangeAsync(int startPort, int endPort, byte[]? payload = null)
    {
        if (startPort < 1 || endPort > 65535 || startPort > endPort)
            throw new ArgumentOutOfRangeException("端口范围无效");

        var results = new List<PortScanResult>();
        var tasks = new List<Task>();
        using var semaphore = new SemaphoreSlim(MaxConcurrency);

        for (int port = startPort; port <= endPort; port++)
        {
            await semaphore.WaitAsync();
            int p = port;

            var task = Task.Run(async () =>
            {
                try
                {
                    var res = await ScanPortAsync(p, payload);
                    lock (results) results.Add(res);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        results.Sort((a, b) => a.Port.CompareTo(b.Port));
        return results;
    }

    /// <summary>
    /// 批量扫描指定端口列表
    /// </summary>
    /// <param name="ports">端口列表</param>
    /// <param name="payload">自定义发送数据</param>
    /// <returns>扫描结果列表</returns>
    public async Task<List<PortScanResult>> ScanPortsAsync(IEnumerable<int> ports, byte[]? payload = null)
    {
        var portList = new List<int>(ports);
        portList.Sort();
        return await ScanRangeAsync(portList[0], portList[^1], payload); // 简化处理
    }

}