using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 抽象基类，定义通用扫描接口
/// </summary>
public abstract class PortScannerBase
{
    /// <summary>目标主机名或 IP 地址</summary>
    public string Host { get; set; }

    /// <summary>扫描起始端口</summary>
    public int StartPort { get; set; } = 1;

    /// <summary>扫描结束端口</summary>
    public int EndPort { get; set; } = 1024;

    /// <summary>超时时间（毫秒）</summary>
    public int Timeout { get; set; } = 500;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="host">目标主机名或 IP</param>
    protected PortScannerBase(string host)
    {
        Host = host;
    }

    /// <summary>
    /// 扫描单个端口（异步）
    /// </summary>
    /// <param name="port">目标端口</param>
    /// <returns>端口扫描结果</returns>
    public abstract Task<PortScanResult> ScanPortAsync(int port);

    /// <summary>
    /// 扫描端口范围（异步）
    /// </summary>
    /// <returns>扫描结果列表</returns>
    public virtual async Task<List<PortScanResult>> ScanRangeAsync()
    {
        var results = new List<PortScanResult>();
        for (int port = StartPort; port <= EndPort; port++)
        {
            results.Add(await ScanPortAsync(port));
        }
        return results;
    }

    /// <summary>
    /// 扫描单个端口（同步）
    /// </summary>
    /// <param name="port">目标端口</param>
    /// <returns>端口扫描结果</returns>
    public PortScanResult ScanPort(int port)
    {
        return ScanPortAsync(port).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 扫描端口范围（同步）
    /// </summary>
    /// <returns>扫描结果列表</returns>
    public List<PortScanResult> ScanRange()
    {
        return ScanRangeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 将扫描结果导出为 JSON 文件，失败返回 false。
    /// </summary>
    /// <param name="results">扫描结果列表</param>
    /// <param name="path">JSON 保存路径</param>
    /// <returns>成功返回 true，失败返回 false</returns>
    public bool ExportJsonSafe(List<PortScanResult> results, string path, out string? errorMessage)
    {
        try
        {
            System.Text.Json.JsonSerializerOptions options = new() { WriteIndented = true };
            var json = System.Text.Json.JsonSerializer.Serialize(results, options);
            File.WriteAllText(path, json, Encoding.UTF8);
            errorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// 将扫描结果导出为 CSV 文件，失败返回 false。
    /// </summary>
    /// <param name="results">扫描结果列表</param>
    /// <param name="path">CSV 保存路径</param>
    /// <returns>成功返回 true，失败返回 false</returns>
    public bool ExportCsvSafe(List<PortScanResult> results, string path)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("Port,Status,Service,ElapsedMilliseconds,ErrorMessage");
            foreach (var r in results)
            {
                sb.AppendLine($"{r.Port},{r.Status},{r.Service},{r.ElapsedMilliseconds},{r.ErrorMessage}");
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
