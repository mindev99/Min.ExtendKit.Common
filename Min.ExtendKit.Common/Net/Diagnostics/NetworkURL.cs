using System.Diagnostics;
using System.Net;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;

namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 提供 HTTP/HTTPS URL链接访问测试功能。
/// </summary>
public class NetworkURL
{
    /// <summary>
    /// 检测 URL 是否可访问，返回 HTTP 状态码。
    /// </summary>
    /// <param name="url">目标 URL。</param>
    /// <param name="timeout">超时时间（毫秒）。</param>
    /// <param name="onError">异常回调。</param>
    /// <returns>HTTP 状态码，失败返回 null。</returns>
    public static async Task<HttpStatusCode?> CheckUrlStatusAsync(string url, int timeout = 5000, Action<Exception>? onError = null)
    {
        url = NormalizeUrl(url);
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(timeout) };
            var response = await client.GetAsync(url);
            return response.StatusCode;
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
            return null;
        }
    }

    /// <summary>
    /// 检测 URL 是否可访问，返回详细结果。
    /// </summary>
    /// <param name="url">目标 URL。</param>
    /// <param name="timeout">超时时间（毫秒）。</param>
    /// <param name="method">HTTP 请求方法（GET/HEAD/POST 等）。</param>
    /// <param name="headers">可选自定义请求头。</param>
    /// <param name="onError">异常回调。</param>
    /// <returns>返回检测结果对象。</returns>
    public static async Task<NetworkUrlResult> CheckUrlSimpleAsync(string url, int timeout = 5000, HttpMethod? method = null, Dictionary<string, string>? headers = null, Action<Exception>? onError = null)
    {
        return await CheckUrlAsync(url, timeout, method, headers, null, true, onError);
    }

    /// <summary>
    /// 检测 URL 是否可访问，并返回详细结果。
    /// </summary>
    /// <param name="url">目标 URL。</param>
    /// <param name="timeout">超时时间（毫秒）。</param>
    /// <param name="method">HTTP 请求方法（GET、POST、HEAD 等）。</param>
    /// <param name="headers">可选自定义请求头。</param>
    /// <param name="proxy">可选 HTTP/HTTPS 代理。
    /// <code>
    /// System.Net.IWebProxy proxy = new System.Net.WebProxy("http://127.0.0.1:8888") 
    /// { 
    ///     Credentials = new System.Net.NetworkCredential("username", "password")  
    /// };
    /// </code>
    /// </param>
    /// <param name="validateCertificate">是否验证 HTTPS 证书有效性。</param>
    /// <param name="onError">异常回调。</param>
    /// <returns>返回检测结果对象。</returns>
    public static async Task<NetworkUrlResult> CheckUrlAsync(string url, int timeout = 5000, HttpMethod? method = null, Dictionary<string, string>? headers = null, IWebProxy? proxy = null, bool validateCertificate = true, Action<Exception>? onError = null)
    {
        var result = new NetworkUrlResult { Url = url };
        method ??= HttpMethod.Get;

        try
        {
            // 提取 host 并解析 IP
            var uri = new Uri(NormalizeUrl(url));
            result.ResolvedIPs = [.. (await Dns.GetHostAddressesAsync(uri.Host))];

            var handler = new HttpClientHandler();
            if (proxy != null) handler.Proxy = proxy;
            handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
            {
                result.CertificateValid = cert != null && (errors == System.Net.Security.SslPolicyErrors.None);
                return validateCertificate ? errors == System.Net.Security.SslPolicyErrors.None : true;
            };

            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(timeout) };
            using var request = new HttpRequestMessage(method, url);

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                }
            }

            var response = await client.SendAsync(request);
            result.StatusCode = response.StatusCode;
            result.IsSuccess = response.IsSuccessStatusCode;
            result.ContentLength = response.Content.Headers.ContentLength ?? 0;

            // 会占用内存
            //var content = await response.Content.ReadAsByteArrayAsync();
            //result.ContentLength = content.Length;
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
            result.Exception = ex;
            result.IsSuccess = false;
        }

        return result;
    }

    /// <summary>
    /// 批量检测 URL 可访问性。
    /// </summary>
    /// <param name="urls">URL 列表。(自动过滤空白和重复)</param>
    /// <param name="timeout">超时时间（毫秒）。</param>
    /// <param name="method">HTTP 请求方法。</param>
    /// <param name="headers">可选请求头。</param>
    /// <param name="proxy">可选代理。</param>
    /// <param name="validateCertificate">是否验证 HTTPS 证书有效性。</param>
    /// <param name="maxConcurrency">最大并发数（默认 10）。</param>
    /// <param name="onError">异常回调。</param>
    /// <returns>返回检测结果列表。</returns>
    public static async Task<List<NetworkUrlResult>> CheckUrlsAsync(IEnumerable<string> urls, int timeout = 5000, HttpMethod? method = null, Dictionary<string, string>? headers = null, IWebProxy? proxy = null, bool validateCertificate = true, int maxConcurrency = 10, Action<Exception>? onError = null)
    {
        // 过滤空白或重复 URL
        urls = urls.Where(u => !string.IsNullOrWhiteSpace(u)).Distinct();

        var results = new List<NetworkUrlResult>();
        using var semaphore = new SemaphoreSlim(maxConcurrency);
        var tasks = urls.Select(async url =>
        {
            await semaphore.WaitAsync();
            try
            {
                var result = await CheckUrlAsync(url, timeout, method, headers, proxy, validateCertificate, onError);
                lock (results) results.Add(result); // 确保线程安全
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return results;
    }

    /// <summary>
    /// 检测 HTTPS 证书有效性，仅适用于 HTTPS 链接或域名。
    /// </summary>
    /// <param name="urlOrHost">HTTPS URL 或域名（例如：https://www.mindev.cn）。</param>
    /// <param name="timeout">超时时间（毫秒）。</param>
    /// <param name="onError">异常回调。</param>
    /// <returns>返回证书是否有效。</returns>
    public static async Task<bool> CheckHttpsCertificateAsync(string urlOrHost, int timeout = 5000, Action<Exception>? onError = null)
    {
        // 如果是裸域名或 http 开头，将其转换为 https:// 前缀
        string url;
        if (urlOrHost.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = urlOrHost;
        }
        else
        {
            urlOrHost = urlOrHost.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? urlOrHost["http://".Length..] : urlOrHost;
            url = $"https://{urlOrHost}";
        }

        var result = await CheckUrlAsync(url, timeout, validateCertificate: true, onError: onError);
        return result.CertificateValid;
    }

    /// <summary>
    /// 获取指定 URL 的 favicon 图标数据
    /// </summary>
    /// <param name="url">目标网站 URL 或域名</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    /// <param name="onError">异常回调</param>
    /// <returns>返回 favicon 数据字节数组，如果未找到返回 null</returns>
    public static async Task<byte[]?> GetFaviconAsync(string url, int timeout = 5000, Action<Exception>? onError = null)
    {
        try
        {
            url = NormalizeUrl(url);
            var uri = new Uri(url);
            var baseUri = $"{uri.Scheme}://{uri.Host}";

            Debug.WriteLine($"BaseUri：{baseUri}");

            using var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(timeout) };

            // 尝试标准 favicon.ico 路径
            var icoUrl = $"{baseUri}/favicon.ico";
            try
            {
                var icoData = await client.GetByteArrayAsync(icoUrl);
                if (icoData.Length > 0) return icoData;
            }
            catch { /* 忽略 */ }

            // 尝试解析 HTML
            var html = await client.GetStringAsync(baseUri);
            var match = System.Text.RegularExpressions.Regex.Match(html, @"<link[^>]+rel=[""'](?:shortcut icon|icon)[""'][^>]*href=[""']([^""']+)[""']", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var href = match.Groups[1].Value;
                string faviconFullUrl;
                if (Uri.TryCreate(href, UriKind.Absolute, out var absUri))
                    faviconFullUrl = absUri.ToString();
                else
                    faviconFullUrl = new Uri(new Uri(baseUri), href).ToString();

                return await client.GetByteArrayAsync(faviconFullUrl);
            }
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
        }

        return null;
    }

    /// <summary>
    /// 规范化 URL：如果缺少协议则默认添加 http://
    /// </summary>
    private static string NormalizeUrl(string url)
    {
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = "http://" + url.Trim();
        }
        return url;
    }

}

