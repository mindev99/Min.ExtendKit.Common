using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// TCP 端口扫描器，提供单端口扫描、端口范围扫描和端口列表扫描功能。
/// 支持异步扫描、并发控制、扫描结果回调、结果导出。
/// </summary>
public class TcpPortScanner : PortScannerBase
{
    /// <summary>
    /// 并发扫描最大线程数，默认 50。
    /// </summary>
    public int MaxConcurrency { get; set; } = 50;

    /// <summary>
    /// 扫描结果回调事件，扫描每个端口完成后触发。
    /// </summary>
    public event Action<PortScanResult>? OnPortScanned;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="host">目标主机（IP 或域名）</param>
    public TcpPortScanner(string host) : base(host) { }

    /// <summary>
    /// 扫描单个 TCP 端口（异步）
    /// </summary>
    /// <param name="port">端口号</param>
    /// <returns>端口扫描结果</returns>
    public override async Task<PortScanResult> ScanPortAsync(int port)
    {
        //var result = new PortScanResult { Port = port };
        //var sw = Stopwatch.StartNew();
        //try
        //{
        //    using var client = new TcpClient();
        //    var task = client.ConnectAsync(Host, port);
        //    if (await Task.WhenAny(task, Task.Delay(Timeout)) == task && client.Connected)
        //    {
        //        result.Status = PortStatus.Open;
        //        result.Service = Service.ServiceIdentifier.GetServiceName(port);
        //    }
        //    else
        //    {
        //        result.Status = PortStatus.Closed;
        //    }
        //}
        //catch (Exception ex)
        //{
        //    result.Status = PortStatus.Error;
        //    result.ErrorMessage = ex.Message;
        //}
        //finally
        //{
        //    sw.Stop();
        //    result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        //}
        //return result;

        return await ScanPortAsync(port, null);
    }

    /// <summary>
    /// 扫描单个 TCP 端口（异步）。
    /// </summary>
    /// <param name="port">端口号</param>
    /// <param name="payload">可选自定义发送数据</param>
    /// <returns>端口扫描结果对象</returns>
    public async Task<PortScanResult> ScanPortAsync(int port, byte[]? payload = null)
    {
        var result = new PortScanResult { Port = port };
        var sw = Stopwatch.StartNew();
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(Host, port);

            if (await Task.WhenAny(connectTask, Task.Delay(Timeout)) == connectTask && client.Connected)
            {
                result.Status = PortStatus.Open;

                // 可选发送自定义 Payload
                if (payload != null && payload.Length > 0)
                {
                    try
                    {
                        var stream = client.GetStream();
                        await stream.WriteAsync(payload, 0, payload.Length);
                    }
                    catch
                    {
                        // 不影响端口状态
                    }
                }

                result.Service = Service.ServiceIdentifier.GetServiceName(port);
            }
            else
            {
                result.Status = PortStatus.Closed;
            }
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
            OnPortScanned?.Invoke(result);
        }
        return result;
    }

    /// <summary>
    /// 批量扫描端口范围（异步）。
    /// </summary>
    /// <param name="startPort">起始端口</param>
    /// <param name="endPort">结束端口</param>
    /// <param name="payload">可选自定义发送数据</param>
    /// <returns>扫描结果列表</returns>
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
    /// 扫描指定端口列表（异步）。
    /// </summary>
    /// <param name="ports">端口列表</param>
    /// <param name="payload">可选自定义发送数据</param>
    /// <returns>扫描结果列表</returns>
    public async Task<List<PortScanResult>> ScanPortsAsync(IEnumerable<int> ports, byte[]? payload = null)
    {
        //var portList = ports.ToList();
        //portList.Sort();
        //return await ScanRangeAsync(portList[0], portList[^1], payload); // 简化处理

        var results = new List<PortScanResult>();
        foreach (var p in ports.ToList())
        {
            results.Add(await ScanPortAsync(p, payload));
        }
        return results;
    }

}