using System.Runtime.InteropServices;

using Min.ExtendKit.Common.Core.Struct;

namespace Min.ExtendKit.Common.Core.Win32.API;

/// <summary>
/// 提供对 Windows Shell API 的托管声明。
/// </summary>
internal partial class DialogAPI
{
    /// <summary>
    /// 从路径字符串创建一个 Shell 项（IShellItem）。
    /// </summary>
    /// <param name="pszPath">文件或文件夹的完整路径（支持 Unicode）。</param>
    /// <param name="pbc">绑定上下文（通常为 IntPtr.Zero）。</param>
    /// <param name="riid">请求的接口 ID，一般为 IShellItem 的 IID。</param>
    /// <param name="ppv">返回的接口指针。</param>
    /// <remarks>
    /// 常用于从路径获取 IShellItem 对象。需要调用 Marshal.Release 或相应封装释放 ppv。
    /// </remarks>
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);

    /// <summary>
    /// 显示一个标准的文件夹浏览对话框。
    /// </summary>
    /// <param name="lpbi">指向 BROWSEINFO 结构体的引用，定义对话框的属性。</param>
    /// <returns>返回所选文件夹的 PIDL（项目 ID 列表）。</returns>
    /// <remarks>
    /// 返回的 PIDL 需要调用 <see cref="CoTaskMemFree"/> 释放。
    /// </remarks>
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

    /// <summary>
    /// 将项目 ID 列表（PIDL）转换为文件系统路径。
    /// </summary>
    /// <param name="pidl">要转换的 PIDL。</param>
    /// <param name="pszPath">接收路径的缓冲区指针。</param>
    /// <returns>如果成功返回 true，否则返回 false。</returns>
    /// <remarks>
    /// 缓冲区大小应至少为 MAX_PATH。常与 <see cref="SHBrowseForFolder"/> 一起使用。
    /// </remarks>
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    internal static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    /// <summary>
    /// 检索指定特殊文件夹的 PIDL。
    /// </summary>
    /// <param name="hwndOwner">父窗口句柄。</param>
    /// <param name="nFolder">CSIDL 常量，指定特殊文件夹。</param>
    /// <param name="ppidl">返回的 PIDL 指针。</param>
    /// <returns>HRESULT。</returns>
    /// <remarks>
    /// 需要使用 <see cref="CoTaskMemFree"/> 释放返回的 PIDL。
    /// CSIDL 在较新系统中已被 KNOWNFOLDERID 取代。
    /// </remarks>
    [DllImport("shell32.dll")]
    internal static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, out IntPtr ppidl);

    /// <summary>
    /// 释放由 COM 分配的内存。
    /// </summary>
    /// <param name="ptr">要释放的指针。</param>
    /// <remarks>
    /// 常用于释放由 Shell API 返回的 PIDL 内存。
    /// </remarks>
    [DllImport("ole32.dll")]
    internal static extern void CoTaskMemFree(IntPtr ptr);

    /// <summary>
    /// 调用 Windows 原生的文件保存对话框（SaveFileDialog 的底层实现）。
    /// </summary>
    /// <param name="ofn">
    /// 指向 OPENFILENAME 结构体的引用，用于设置对话框的属性和获取用户选择的结果。
    /// 在调用前需要填充结构体的相关字段，如：
    /// - lpstrFile: 文件路径缓冲区
    /// - nMaxFile: 缓冲区大小
    /// - lpstrFilter: 文件类型过滤器
    /// - Flags: 对话框行为标志
    /// 调用后，ofn.lpstrFile 中会保存用户选择的文件路径。
    /// </param>
    /// <returns>
    /// 返回 true 表示用户点击了“保存”并选择了文件；
    /// 返回 false 表示用户取消了对话框或出现错误。
    /// 出错时可通过 Marshal.GetLastWin32Error() 获取详细错误码。
    /// </returns>
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool GetSaveFileName(ref OPENFILENAME ofn);

    /// <summary>
    /// 调用 Win32 API 显示系统颜色选择对话框。
    /// </summary>
    /// <param name="cc">
    /// <see cref="CHOOSECOLOR"/> 结构体的引用，包含对话框初始化参数，
    /// 并在用户完成选择后返回所选颜色和自定义颜色数组。
    /// </param>
    /// <returns>
    /// 如果用户点击“确定”并选择颜色，返回 <c>true</c>；
    /// 如果用户点击“取消”或调用失败，返回 <c>false</c>。
    /// </returns>
    /// <remarks>
    /// 1. 该函数定义于 <c>comdlg32.dll</c> 中。  
    /// 2. 调用前必须将 <see cref="CHOOSECOLOR.lStructSize"/> 设置为 <c>Marshal.SizeOf(typeof(CHOOSECOLOR))</c>。  
    /// 3. 通过 <see cref="CHOOSECOLOR.Flags"/> 控制对话框行为（如 <c>CC_RGBINIT</c>, <c>CC_FULLOPEN</c>）。  
    /// 4. 成功返回后，所选颜色存储在 <see cref="CHOOSECOLOR.rgbResult"/> 字段中，
    /// 格式为 0x00BBGGRR（低字节为 R，高字节为 B）。  
    /// 5. 可通过 <see cref="Marshal.GetLastWin32Error"/> 获取详细错误码。  
    /// </remarks>
    [DllImport("comdlg32.dll", SetLastError = true)]
    public static extern bool ChooseColor(ref CHOOSECOLOR cc);

}
