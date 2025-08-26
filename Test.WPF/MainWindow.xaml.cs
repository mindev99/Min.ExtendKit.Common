using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using Min.ExtendKit.Common.Dialogs;
using Min.ExtendKit.Common.Net.Diagnostics;
using Min.ExtendKit.Common.SysInfo;

namespace Test.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        //string? t = string.Empty;
        //t = FolderDialog.ModernBrowseForFolder("[Modern] 选择文件夹", null, onError: (msg, ex) =>
        //{
        //    MessageBox.Show($"{msg}\n{ex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        //},new WindowInteropHelper(this).Handle);

        //t = FolderDialog.ClassicBrowseForFolder("[Classic] 选择文件夹", onError: (msg, ex) =>
        //{
        //    MessageBox.Show($"{msg}\n{ex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        //}, new WindowInteropHelper(this).Handle);

        //var filters = new COMDLG_FILTERSPEC[]
        //{
        //    new() { pszName = "文本文件 (*.txt)", pszSpec = "*.txt" },
        //    new() { pszName = "所有文件 (*.*)",   pszSpec = "*.*"  }
        //};
        //string[]? array = Min.ExtendKit.Common.Dialogs.FileDialog.OpenFile("打开文件", defaultPath: @"C:\", allowMultiSelect: true, filter: filters, winHandle: new WindowInteropHelper(this).Handle);
        //if (array != null)
        //{
        //    MessageBox.Show(string.Join(Environment.NewLine, array));
        //}

        //string? path = Min.ExtendKit.Common.Dialogs.FileDialog.SaveFile(title: "请选择保存位置", defaultPath: @"C:\", defaultFileName: "test.txt", filter: filters, onError: (msg, stack) => Console.WriteLine($"错误: {msg} - {stack}"), winHandle: new WindowInteropHelper(this).Handle);

        //MessageBox.Show(path);

        //MinColor? selectedcolor = ColorDialog.Show(new WindowInteropHelper(this).Handle, new MinColor(255, 0, 0));
        //if (selectedcolor.HasValue)
        //{
        //    MinColor color = selectedcolor.Value;
        //    MessageBox.Show($"选择的颜色: R={color.R}, G={color.G}, B={color.B}, A={color.A}");
        //}

        //Debug.WriteLine("\n=== UserInfo 测试输出 ===");

        //Debug.WriteLine($"用户名 UserName: {UserInfo.UserName}");
        //Debug.WriteLine($"用户域名 UserDomainName: {UserInfo.UserDomainName}");
        //Debug.WriteLine($"用户 SID UserSID: {UserInfo.UserSID}");
        //Debug.WriteLine($"是否管理员 IsAdministrator: {UserInfo.IsAdministrator}");
        //Debug.WriteLine($"用户主目录 UserProfilePath: {UserInfo.UserProfilePath}");
        //Debug.WriteLine($"文档目录 MyDocumentsPath: {UserInfo.MyDocumentsPath}");
        //Debug.WriteLine($"桌面目录 DesktopPath: {UserInfo.DesktopPath}");
        //Debug.WriteLine($"下载目录 DownloadsPath: {UserInfo.DownloadsPath}");
        //Debug.WriteLine($"临时目录 TempPath: {UserInfo.TempPath}");
        //Debug.WriteLine($"应用数据目录 AppDataPath: {UserInfo.AppDataPath}");
        //Debug.WriteLine($"本地应用数据目录 LocalAppDataPath: {UserInfo.LocalAppDataPath}");
        //Debug.WriteLine($"当前目录 CurrentDirectory: {UserInfo.CurrentDirectory}");
        //Debug.WriteLine($"程序路径 ExecutablePath: {UserInfo.ExecutablePath}");

        //Debug.WriteLine($"本机 IPv4 LocalIPv4: {string.Join(", ", UserInfo.LocalIPv4)}");
        //Debug.WriteLine($"网卡 MACAddresses: {string.Join(", ", UserInfo.MACAddresses)}");

        //Debug.WriteLine($"系统 PATH PATH: {UserInfo.PATH}");
        //Debug.WriteLine($"系统 TEMP TEMP: {UserInfo.TEMP}");
        //Debug.WriteLine($"APPDATA APPDATA: {UserInfo.APPDATA}");

        //Debug.WriteLine("=== 输出结束 ===\n");


        //Debug.WriteLine("\n====== 操作系统信息调试输出 ======");

        //Debug.WriteLine($"操作系统描述: {OSInfo.Description}");
        //Debug.WriteLine($"操作系统架构: {OSInfo.Architecture}");
        //Debug.WriteLine($"操作系统版本: {OSInfo.OSVersion}");
        //Debug.WriteLine($"操作系统平台: {OSInfo.Platform}");
        //Debug.WriteLine($"Windows SKU: {OSInfo.WindowsSKU}");
        //Debug.WriteLine($"是否 64 位: {OSInfo.Is64Bit}");
        //Debug.WriteLine($"系统启动时间: {OSInfo.BootTime}");
        //Debug.WriteLine($"系统页大小: {OSInfo.SystemPageSize} 字节");
        //Debug.WriteLine($"计算机名: {OSInfo.MachineName}");
        //Debug.WriteLine($"系统语言: {OSInfo.SystemLanguage}");
        //Debug.WriteLine($"用户界面语言: {OSInfo.UserInterfaceLanguage}");
        //Debug.WriteLine($"时区: {OSInfo.TimeZone}");
        //Debug.WriteLine($"当前线程管理 ID: {OSInfo.SessionId}");

        //Debug.WriteLine("====== 调试输出结束 ======\n");



        //Debug.WriteLine("\n=== CPU 信息调试输出 ===");
        //Debug.WriteLine($"逻辑处理器数量 (ProcessorCount): {CpuInfo.ProcessorCount}");
        //Debug.WriteLine($"CPU 架构 (Architecture): {CpuInfo.Architecture}");
        //Debug.WriteLine($"逻辑核心数量 (LogicalProcessorCount): {CpuInfo.LogicalProcessorCount}");
        //Debug.WriteLine($"最大频率估算 (MaxClockSpeedMHz): {CpuInfo.MaxClockSpeedMHz} MHz");
        //Debug.WriteLine("=========================\n");

        //Debug.WriteLine("\n=== 内存信息调试输出 ===");
        //Debug.WriteLine($"总物理内存 (TotalPhysicalMemory): {MemoryInfo.TotalPhysicalMemory} 字节");
        //Debug.WriteLine($"可用物理内存 (AvailablePhysicalMemory): {MemoryInfo.AvailablePhysicalMemory} 字节");
        //Debug.WriteLine($"已用物理内存 (UsedPhysicalMemory): {MemoryInfo.UsedPhysicalMemory} 字节");
        //Debug.WriteLine($"物理内存使用率 (PhysicalMemoryLoad): {MemoryInfo.PhysicalMemoryLoad}%");
        //Debug.WriteLine($"总虚拟内存 (TotalVirtualMemory): {MemoryInfo.TotalVirtualMemory} 字节");
        //Debug.WriteLine($"可用虚拟内存 (AvailableVirtualMemory): {MemoryInfo.AvailableVirtualMemory} 字节");
        //Debug.WriteLine($"已用虚拟内存 (UsedVirtualMemory): {MemoryInfo.UsedVirtualMemory} 字节");
        //Debug.WriteLine("==========================\n");

        //Debug.WriteLine(CpuInfo.GetManufacturer());

        // Debug.WriteLine(NetworkInfo.GetMacAddress());
        // Debug.WriteLine(NetworkInfo.GetDebugInfo());



        // ------------------------------
        // 1️⃣ 测试同步 Ping
        // ------------------------------
        //string hostSync = "8.8.8.8";
        //string syncResult = NetworkPing.PingHost(hostSync, 3, 1000);
        //Debug.WriteLine("=== 同步 Ping ===");
        //Debug.WriteLine(syncResult);

        // ------------------------------
        // 2️⃣ 测试异步 Ping
        // ------------------------------
        //string hostAsync = "1.1.1.1";
        //var asyncReport = await NetworkPing.PingHostAsync(hostAsync, 4, 1000);
        //Debug.WriteLine("=== 异步 Ping ===");
        //Debug.WriteLine(asyncReport.ToString());

        // ------------------------------
        // 3️⃣ 测试快速诊断 QuickPingAsync
        // ------------------------------
        //var (lossRate, avgLatency) = await NetworkPing.QuickPingAsync("localhost", 3, 500);
        //Debug.WriteLine("=== 快速诊断 ===");
        //Debug.WriteLine($"Loss Rate: {lossRate:F1}%  Avg Latency: {avgLatency}ms");

        // ------------------------------
        // 4️⃣ 测试全局 Ping，可自定义 hosts
        // ------------------------------
        //string[] customHosts = { "localhost", "api.ipify.org", "www.baidu.com" };
        //Debug.WriteLine("=== 自定义全局 Ping ===");
        //await NetworkPing.PingMultipleHostsAsync(customHosts, CallBack);

        // ------------------------------
        // 5️⃣ 测试统计信息
        // ------------------------------
        //var pingTimes = await NetworkPing.PingAsync("8.8.8.8", 5, 1000);
        //var (success, fail, loss, avg) = NetworkPing.GetPingStatistics(pingTimes);
        //Debug.WriteLine("=== Ping 统计信息 ===");
        //Debug.WriteLine($"Sent: 5, Success: {success}, Fail: {fail}, Loss: {loss:F1}%, Avg: {avg}ms");

        // TestTracerouteDetailed(this.TestButton);

        // await TestUdpScanAsync("127.0.0.1", 1, 1024);
        await TestTcpPortScannerAsync();

    }

    private void CallBack(string test)
    {
        Debug.WriteLine(test);
    }

    private async void TestNetworkTracerouteAll()
    {
        var hosts = new string[] { "localhost", "github.com", "www.baidu.com" };

        foreach (var host in hosts)
        {
            Debug.WriteLine($"=== Testing Host: {host} ===");

            // 1. 同步 Traceroute，获取文本报告
            string simpleReport = NetworkTraceroute.TracerouteSimple(host);
            Debug.WriteLine("TracerouteSimple report:\n" + simpleReport);

            // 2. 同步 Traceroute，获取每跳列表
            var hopsSync = NetworkTraceroute.TracerouteHops(host);
            Debug.WriteLine("TracerouteHops (sync) report:\n" + NetworkTraceroute.ToReport(hopsSync));

            // 3. 异步 Traceroute
            var hopsAsync = await NetworkTraceroute.TracerouteAsync(host);
            Debug.WriteLine("TracerouteAsync report:\n" + NetworkTraceroute.ToReport(hopsAsync));

            // 4. RTT 统计
            Debug.WriteLine($"Max RTT: {NetworkTraceroute.MaxRTT(hopsAsync)} ms");
            Debug.WriteLine($"Min RTT: {NetworkTraceroute.MinRTT(hopsAsync)} ms");
            Debug.WriteLine($"Avg RTT: {NetworkTraceroute.AvgRTT(hopsAsync):F1} ms");

            // 5. 异常节点检测
            var anomalies = NetworkTraceroute.DetectAnomalies(hopsAsync);
            Debug.WriteLine("Anomaly hops: " + (anomalies.Count > 0 ? string.Join(", ", anomalies) : "None"));

            // 6. 检查是否经过私有 IP
            Debug.WriteLine("Passed private IP: " + NetworkTraceroute.PassedPrivateIP(hopsAsync));

            // 7. 快速 Traceroute 测试
            var quickResult = await NetworkTraceroute.QuickTracerouteAsync(host);
            Debug.WriteLine($"QuickTraceroute: Avg RTT={quickResult.Avg:F1} ms, Max RTT={quickResult.Max} ms");

            // 8. 导出 CSV 测试（临时导出到当前目录）
            string csvPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{host}_traceroute.csv");
            NetworkTraceroute.ExportToCsv(csvPath, hopsAsync);
            Debug.WriteLine("Exported CSV to: " + csvPath);

            Debug.WriteLine($"=== Finished Host: {host} ===\n\n");
        }

        // 9. 批量 Traceroute 测试
        Debug.WriteLine("=== Batch Traceroute Test ===");
        await NetworkTraceroute.BatchTracerouteAsync(hosts, s => Debug.WriteLine(s));

        // 10. 多目标 Traceroute
        var multiHops = await NetworkTraceroute.TracerouteMultipleAsync(hosts);
        foreach (var kv in multiHops)
        {
            Debug.WriteLine($"Host: {kv.Key}, hops count: {kv.Value.Count}");
        }

        Debug.WriteLine("All NetworkTraceroute tests complete!");
    }

    private async void TestTracerouteDetailed(Button Button_Trace)
    {
        Button_Trace.IsEnabled = false; // 禁用按钮

        string targetHost = "www.baidu.com";
        int port = 80;
        string csvPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "trace_result.csv");

        // 创建实例
        var trace = new DetailedTrace(targetHost, port: port, maxHops: 20, timeout: 3000, retryPerHop: 2);

        // 注册回调：每跳完成时实时打印到控制台
        trace.OnHopCompleted += hop =>
        {
            Debug.WriteLine($"[Callback] Hop {hop.Hop}: IP={hop.IP}, Hostname={hop.Hostname}, Status={hop.Status}, RTT={hop.RTT}ms");
        };

        try
        {
            // 1. TCP 测试
            Debug.WriteLine("=== TCP Trace ===");
            await trace.TraceAsync(DetailedTrace.TraceMode.TCP);
            Debug.WriteLine(trace.ToReport());

            // 2. ICMP 测试
            Debug.WriteLine("=== ICMP Trace ===");
            await trace.TraceAsync(DetailedTrace.TraceMode.ICMP);
            Debug.WriteLine(trace.ToReport());

            // 3. 保存 CSV
            trace.SaveReportCsv(csvPath);
            Debug.WriteLine($"Trace results saved to CSV: {csvPath}");

            // 4. 检查可达性
            bool reachable = trace.IsReachable();
            Debug.WriteLine($"Is Reachable: {reachable}");

            // 5. 弹窗显示（可选）
            MessageBox.Show(trace.ToReport(), $"Trace Result - {targetHost}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Trace failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Button_Trace.IsEnabled = true; // 恢复按钮
        }
    }


    /// <summary>
    /// 测试 UDP 端口扫描器，扫描指定范围并导出 JSON
    /// </summary>
    /// <param name="host">目标主机</param>
    /// <param name="startPort">起始端口</param>
    /// <param name="endPort">结束端口</param>
    /// <returns>异步任务</returns>
    public static async Task TestUdpScanAsync(string host, int startPort = 1, int endPort = 1024)
    {
        try
        {
            var scanner = new UdpPortScanner("localhost")
            {
                Timeout = 1000,
                MaxConcurrency = 20
            };

            // 1. 测试单个端口扫描
            var singleResult = await scanner.ScanPortAsync(53); // DNS 常用 UDP 端口
            Debug.WriteLine($"[单端口测试] {singleResult.Port} => {singleResult.Status} ({singleResult.Service})");

            // 2. 测试范围扫描
            var rangeResults = await scanner.ScanRangeAsync(50, 60);
            Debug.WriteLine("[范围扫描测试]");
            foreach (var r in rangeResults)
                Debug.WriteLine($"{r.Port} => {r.Status} ({r.Service})");

            // 3. 测试指定端口列表扫描
            var listResults = await scanner.ScanPortsAsync(new[] { 67, 68, 123 }); // DHCP/NTP
            Debug.WriteLine("[端口列表扫描测试]");
            foreach (var r in listResults)
                Debug.WriteLine($"{r.Port} => {r.Status} ({r.Service})");

            // 4. 测试 JSON 导出
            var exportSuccess = scanner.ExportJsonSafe(rangeResults, "udp_scan_results.json", out string? errorMsg);
            if (exportSuccess)
                Debug.WriteLine($"[导出 JSON 成功] 文件已保存到 udp_scan_results.json");
            else
                Debug.WriteLine($"[导出 JSON 失败] {errorMsg}");

            scanner.ExportCsvSafe(rangeResults, "udp_scan_results.csv");

            Debug.WriteLine("✅ UdpPortScanner 全部方法测试完成");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ 测试过程中发生异常: {ex.Message}");
        }
    }


    public static async Task TestTcpPortScannerAsync()
    {
        Debug.WriteLine("[开始] TCP 测试");
        string host = "localhost"; // 可以改成你的目标主机
        var scanner = new TcpPortScanner(host)
        {
            Timeout = 1000,
            MaxConcurrency = 20
        };

        // 注册回调事件
        scanner.OnPortScanned += result =>
        {
            Debug.WriteLine($"[TCP 回调] {result.Port} => {result.Status} ({result.Service})");
        };

        Debug.WriteLine("[单端口测试 (80) ]");
        var singleResult80 = await scanner.ScanPortAsync(80); // 测试单个端口
        Debug.WriteLine($"{singleResult80.Port} => {singleResult80.Status} ({singleResult80.Service})");

        Debug.WriteLine("[单端口测试 (8000) ]");
        var singleResult83 = await scanner.ScanPortAsync(8000); // 测试单个端口
        Debug.WriteLine($"{singleResult83.Port} => {singleResult83.Status} ({singleResult83.Service})");

        Debug.WriteLine("[范围扫描测试]");
        var rangeResults = await scanner.ScanRangeAsync(78, 85); // 扫描端口范围
        foreach (var r in rangeResults)
            Debug.WriteLine($"{r.Port} => {r.Status} ({r.Service})");

        Debug.WriteLine("[端口列表扫描测试]");
        var portList = new List<int> { 22, 80, 5040, 443 };
        var listResults = await scanner.ScanPortsAsync(portList); // 扫描指定端口列表
        foreach (var r in listResults)
            Debug.WriteLine($"{r.Port} => {r.Status} ({r.Service})");

        // 导出 JSON

        scanner.ExportJsonSafe(listResults, "tcp_scan_results.json", out string? orrer);
        scanner.ExportCsvSafe(listResults, "tcp_scan_results.csv");

        //string jsonPath = "tcp_scan_results.json";
        //try
        //{
        //    var allResults = rangeResults.Concat(listResults).Prepend(singleResult).ToList();
        //    System.Text.Json.JsonSerializerOptions options = new() { WriteIndented = true };
        //    System.IO.File.WriteAllText(jsonPath, System.Text.Json.JsonSerializer.Serialize(allResults, options));
        //    Console.WriteLine($"[导出 JSON 成功] 文件已保存到 {jsonPath}");
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"[导出 JSON 失败] {ex.Message}");
        //}

        Debug.WriteLine("? TcpPortScanner 全部方法测试完成");

        Debug.WriteLine("[结束] TCP 测试");
    }

    public static async Task TestAllMethodsAsync()
    {
        // 测试 URL 列表
        var urls = new List<string>
        {
            "https://www.baidu.com",
            "http://love.mindev.cn",
            "https://expired.badssl.com/"
        };

        // ================== CheckUrlStatusAsync ==================
        Debug.WriteLine("==== CheckUrlStatusAsync ====");
        foreach (var url in urls)
        {
            var status = await NetworkURL.CheckUrlStatusAsync(url, 5000, ex => Debug.WriteLine($"Error: {ex.Message}"));
            Debug.WriteLine($"URL: {url} => Status: {status}");
        }

        // ================== CheckUrlAsync (简单) ==================
        Debug.WriteLine("==== CheckUrlAsync (简单) ====");
        foreach (var url in urls)
        {
            var result = await NetworkURL.CheckUrlAsync(url, 5000, onError: ex => Debug.WriteLine($"Error: {ex.Message}"));
            Debug.WriteLine($"URL: {url}, Success: {result.IsSuccess}, StatusCode: {result.StatusCode}, ContentLength: {result.ContentLength}");
            if (result.ResolvedIPs.Count > 0)
                Debug.WriteLine($"Resolved IPs: {string.Join(", ", result.ResolvedIPs)}");
        }

        // ================== CheckUrlAsync (带 Headers & Proxy) ==================
        Debug.WriteLine("==== CheckUrlAsync (带 Headers & Proxy) ====");
        IWebProxy? proxy = null; // 可配置代理，例如 new WebProxy("http://127.0.0.1:8888")
        foreach (var url in urls)
        {
            var headers = new Dictionary<string, string>
            {
                { "User-Agent", "NetworkURLTester" }
            };

            var result = await NetworkURL.CheckUrlAsync(url, 5000, headers: headers, proxy: proxy, validateCertificate: true, onError: ex => Debug.WriteLine($"Error: {ex.Message}"));
            Debug.WriteLine($"URL: {url}, Success: {result.IsSuccess}, StatusCode: {result.StatusCode}, CertificateValid: {result.CertificateValid}, ContentLength: {result.ContentLength}");
            if (result.ResolvedIPs.Count > 0)
                Debug.WriteLine($"Resolved IPs: {string.Join(", ", result.ResolvedIPs)}");
        }

        // ================== CheckUrlsAsync (批量) ==================
        Debug.WriteLine("==== CheckUrlsAsync (批量) ====");
        var batchResults = await NetworkURL.CheckUrlsAsync(urls, maxConcurrency: 5, onError: ex => Debug.WriteLine($"Error: {ex.Message}"));
        foreach (var r in batchResults)
        {
            Debug.WriteLine($"URL: {r.Url}, Success: {r.IsSuccess}, StatusCode: {r.StatusCode}, CertificateValid: {r.CertificateValid}, ContentLength: {r.ContentLength}");
            if (r.ResolvedIPs.Count > 0)
                Debug.WriteLine($"Resolved IPs: {string.Join(", ", r.ResolvedIPs)}");
        }

        // ================== CheckHttpsCertificateAsync ==================
        Debug.WriteLine("==== CheckHttpsCertificateAsync ====");
        foreach (var url in urls)
        {
            try
            {
                bool valid = await NetworkURL.CheckHttpsCertificateAsync(url, 5000, ex => Debug.WriteLine($"Error: {ex.Message}"));
                Debug.WriteLine($"URL/Host: {url}, CertificateValid: {valid}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"URL/Host: {url}, Exception: {ex.Message}");
            }
        }

        Debug.WriteLine("==== Test Complete ====");
    }
    private async void Open_Click(object sender, RoutedEventArgs e)
    {
        //await TestNetworkURLAsync();

        await TestAllMethodsAsync();
    }

    private async void Read_Click(object sender, RoutedEventArgs e)
    {
        var faviconData = await NetworkURL.GetFaviconAsync("https://www.teambition.com/project/675aa6576ca2730959247695/tasks/view/675aa657ed5cf3462236cd15");
        Debug.WriteLine($"Favicon Size: {faviconData.Length} bytes");
        if (faviconData == null)
        {
            Debug.WriteLine($"No favicon found for");
            return;
        }
        // 转成 BitmapImage
        var bitmap = new BitmapImage();
        using (var ms = new System.IO.MemoryStream(faviconData))
        {
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze(); // 跨线程安全
        }
        this.Icon.Source = bitmap;
    }
}