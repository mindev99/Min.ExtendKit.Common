using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 提供 DNS 域名解析和反向解析工具。
/// </summary>
public class DNS
{
    /// <summary>
    /// 执行反向 DNS 解析（IP → 域名）。
    /// </summary>
    /// <param name="ip">目标 IP 地址。</param>
    /// <returns>解析到的主机名，失败返回 null。</returns>
    public static string? ReverseDnsLookup(string ip)
    {
        try
        {
            var entry = Dns.GetHostEntry(ip);
            return entry.HostName;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 执行 DNS 域名解析。
    /// </summary>
    /// <param name="host">目标主机名。</param>
    /// <returns>解析得到的 IP 地址数组。</returns>
    public static IPAddress[] DnsResolve(string host)
    {
        try
        {
            return Dns.GetHostAddresses(host);
        }
        catch
        {
            return [];
        }
    }


}
