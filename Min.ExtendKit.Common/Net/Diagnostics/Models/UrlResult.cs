using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 表示 HTTP/HTTPS URL 检测的结果信息。
/// </summary>
public class NetworkUrlResult
{
    /// <summary>
    /// 被检测的 URL 地址。
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// HTTP 响应状态码，如果请求失败则为 null。
    /// </summary>
    public System.Net.HttpStatusCode? StatusCode { get; set; }

    /// <summary>
    /// 请求是否成功（状态码在 200~299 范围内为 true）。
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 响应内容长度（字节数），如果请求失败则为 0。
    /// </summary>
    public long ContentLength { get; set; }

    /// <summary>HTTPS 证书是否有效</summary>
    public bool CertificateValid { get; set; } = false;

    /// <summary>
    /// 获取或设置该 URL 最终解析得到的 IP 地址列表。
    /// 在访问 URL 之前，会通过 DNS 解析域名得到对应的 IP 地址，并保存在此列表中。
    /// 对于单个域名，通常会有多个 IP（IPv4/IPv6），列表顺序与系统 DNS 解析结果一致。
    /// </summary>
    public List<System.Net.IPAddress> ResolvedIPs { get; set; } = [];

    /// <summary>
    /// 请求过程中捕获的异常对象，如果没有异常则为 null。
    /// </summary>
    public Exception? Exception { get; set; }
}
