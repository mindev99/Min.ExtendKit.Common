using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 封装端口扫描结果
/// </summary>
public class PortScanResult
{
    /// <summary>端口号</summary>
    public int Port { get; set; }

    /// <summary>扫描状态</summary>
    public PortStatus Status { get; set; }

    /// <summary>可选的服务标识</summary>
    public string? Service { get; set; }

    /// <summary>扫描异常信息（如果有）</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>扫描耗时（毫秒）</summary>
    public long ElapsedMilliseconds { get; set; }

    public override string ToString() =>
        $"Port: {Port}, Status: {Status}, Service: {Service}, Error: {ErrorMessage}, Time: {ElapsedMilliseconds}ms";
}