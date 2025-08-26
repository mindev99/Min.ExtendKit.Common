namespace Min.ExtendKit.Common.Net.Diagnostics;

/// <summary>
/// 枚举端口状态
/// </summary>
public enum PortStatus
{
    /// <summary>端口开放</summary>
    Open,
    /// <summary>端口关闭</summary>
    Closed,
    /// <summary>端口被防火墙过滤</summary>
    Filtered,
    /// <summary>超时</summary>
    Timeout,
    /// <summary>扫描异常</summary>
    Error
}