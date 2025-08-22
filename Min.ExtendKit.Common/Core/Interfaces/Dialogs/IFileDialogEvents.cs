using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Core.Interfaces.Dialogs;

/// <summary>
/// 用于接收文件对话框事件通知的接口
/// </summary>
/// <remarks>接口GUID: 973510db-7d7f-452b-8975-74a85828d354</remarks>
[ComImport]
[Guid("973510db-7d7f-452b-8975-74a85828d354")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IFileDialogEvents
{
    /// <summary>
    /// 当文件对话框中的文件夹即将改变时触发
    /// </summary>
    /// <param name="pfd">文件对话框接口</param>
    /// <param name="psiFolder">即将切换到的文件夹</param>
    /// <returns>HRESULT结果码，S_OK允许切换，其他值阻止切换</returns>
    [PreserveSig]
    int OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileOpenDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

    /// <summary>
    /// 当文件对话框中的文件夹已改变时触发
    /// </summary>
    /// <param name="pfd">文件对话框接口</param>
    void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileOpenDialog pfd);

    /// <summary>
    /// 当选中的文件类型改变时触发
    /// </summary>
    /// <param name="pfd">文件对话框接口</param>
    void OnFileTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileOpenDialog pfd);

    /// <summary>
    /// 当选中的项目改变时触发
    /// </summary>
    /// <param name="pfd">文件对话框接口</param>
    void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileOpenDialog pfd);

    /// <summary>
    /// 当对话框即将关闭时触发
    /// </summary>
    /// <param name="pfd">文件对话框接口</param>
    /// <param name="hrResult">对话框关闭的结果码</param>
    void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileOpenDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out uint pResponse);

    /// <summary>
    /// 当发生文件共享冲突时触发
    /// </summary>
    /// <param name="pfd">文件对话框接口</param>
    /// <param name="psi">发生冲突的文件</param>
    /// <param name="pResponse">响应方式</param>
    void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileOpenDialog pfd);

    /// <summary>
    /// 当验证文件名称时触发
    /// </summary>
    /// <param name="pfd">文件对话框接口</param>
    /// <param name="pszFileName">要验证的文件名</param>
    /// <returns>HRESULT结果码，S_OK表示验证通过</returns>
    [PreserveSig]
    int OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileOpenDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out uint pResponse);

    /// <summary>
    /// 枚举集合中的下一个 Shell 项。
    /// </summary>
    /// <param name="celt">请求获取的项数，通常为 1。</param>
    /// <param name="item">输出的 <see cref="IShellItem"/> 对象，如果返回成功，则包含当前项。</param>
    /// <param name="fetched">实际返回的项数，0 表示没有更多项，1 表示成功获取 1 项。</param>
    /// <returns>
    /// HRESULT 状态码：
    /// <list type="bullet">
    /// <item><description>0 (S_OK) - 成功获取请求的项</description></item>
    /// <item><description>1 (S_FALSE) - 没有更多项可枚举</description></item>
    /// <item><description>其他 HRESULT - 出错</description></item>
    /// </list>
    /// </returns>
    int Next(int v, out IShellItem item, out uint fetched);
}

