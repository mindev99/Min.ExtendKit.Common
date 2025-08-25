using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Min.ExtendKit.Common.Core.Enum;
using Min.ExtendKit.Common.Dialogs;

namespace Min.ExtendKit.Common.Core.Interfaces.Dialogs;

/// <summary>
/// 表示Windows Shell API中的IFileOpenDialog COM接口，用于创建现代文件打开对话框。
/// 该接口提供了丰富的文件选择功能，支持筛选、预览、多选等高级特性。
/// 继承自IModalWindow和IFileDialog接口，提供标准对话框行为。
/// </summary>
/// <remarks>
/// 接口GUID: D57C7288-D4AD-4768-BE02-9D969532D960
/// 通常通过CreateFileOpenDialog函数获取该接口实例。
/// 适用于Windows Vista及以上系统，提供比传统OpenFileDialog更现代的用户体验。
/// </remarks>
[SupportedOSPlatform("windows")]
[ComImport]
[Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IFileOpenDialog
{
    #region IModalWindow 接口方法

    /// <summary>
    /// 显示模态对话框
    /// </summary>
    /// <param name="hwndParent">对话框的父窗口句柄，如果为IntPtr.Zero则使用桌面窗口</param>
    /// <returns>
    /// 操作结果的HRESULT代码：
    /// - S_OK: 用户点击了确定按钮
    /// - HRESULT_FROM_WIN32(ERROR_CANCELLED): 用户取消了操作
    /// - 其他值表示发生错误
    /// </returns>
    [PreserveSig]
    int Show([In] nint hwndParent);

    #endregion

    // 设置文件过滤规则
    //void SetFileFilterSpec(uint cFileTypes, [In] Dialogs.COMDLG_FILTERSPEC[] rgFilterSpec);

    #region IFileDialog 接口方法

    /// <summary>
    /// 设置对话框中可用的文件类型筛选器
    /// </summary>
    /// <param name="cFileTypes">筛选器的数量，必须大于0</param>
    /// <param name="rgFilterSpec">指向COMDLG_FILTERSPEC结构数组的指针，每个结构定义一个文件类型的显示名称和筛选模式</param>
    /// <remarks>
    /// COMDLG_FILTERSPEC结构定义为：
    /// <code>
    /// [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    /// public struct COMDLG_FILTERSPEC
    /// {
    ///     [MarshalAs(UnmanagedType.LPWStr)] public string pszName;   // 显示名称，如"文本文件"
    ///     [MarshalAs(UnmanagedType.LPWStr)] public string pszSpec;   // 筛选模式，如"*.txt"
    /// }
    /// </code>
    /// 调用前需将托管数组转换为非托管内存块，调用后需释放内存
    /// </remarks>
    void SetFileTypes(uint cFileTypes, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] COMDLG_FILTERSPEC[] rgFilterSpec);

    /// <summary>
    /// 设置默认选中的文件类型索引
    /// </summary>
    /// <param name="iFileType">文件类型索引（从1开始）</param>
    void SetFileTypeIndex([In] uint iFileType);

    /// <summary>
    /// 获取当前选中的文件类型索引
    /// </summary>
    /// <param name="piFileType">输出参数，返回当前选中的文件类型索引（从1开始）</param>
    void GetFileTypeIndex(out uint piFileType);

    /// <summary>
    /// 注册对话框事件通知
    /// </summary>
    /// <param name="pfde">实现IFileDialogEvents接口的对象，用于接收事件通知</param>
    /// <param name="pdwCookie">输出参数，返回标识此通知注册的Cookie值，用于后续取消注册</param>
    void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

    /// <summary>
    /// 取消之前通过Advise方法注册的事件通知
    /// </summary>
    /// <param name="dwCookie">通过Advise方法获取的Cookie值</param>
    void Unadvise([In] uint dwCookie);

    /// <summary>
    /// 设置对话框的行为选项
    /// </summary>
    /// <param name="fos">FOS枚举值的组合，指定对话框的行为选项</param>
    void SetOptions([In] FOS fos);

    /// <summary>
    /// 获取对话框当前的行为选项
    /// </summary>
    /// <param name="pfos">输出参数，返回当前设置的行为选项</param>
    void GetOptions(out FOS pfos);

    /// <summary>
    /// 设置对话框打开时的默认文件夹（不改变当前文件夹）
    /// </summary>
    /// <param name="psi">表示默认文件夹的IShellItem接口</param>
    void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

    /// <summary>
    /// 设置对话框打开时显示的文件夹
    /// </summary>
    /// <param name="psi">表示要显示的文件夹的IShellItem接口</param>
    void SetFolder(nint psi);

    /// <summary>
    /// 获取对话框当前显示的文件夹
    /// </summary>
    /// <param name="ppsi">输出参数，返回表示当前文件夹的IShellItem接口</param>
    void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    /// <summary>
    /// 获取对话框中当前选中的项目
    /// </summary>
    /// <param name="ppsi">输出参数，返回表示当前选中项目的IShellItem接口</param>
    void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    /// <summary>
    /// 设置对话框中文件名编辑框的初始文本
    /// </summary>
    /// <param name="pszName">要显示的初始文件名</param>
    void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

    /// <summary>
    /// 获取对话框中文件名编辑框当前的文本
    /// </summary>
    /// <param name="pszName">输出参数，返回当前的文件名文本</param>
    void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

    /// <summary>
    /// 设置对话框的标题文本
    /// </summary>
    /// <param name="pszTitle">要显示的标题文本</param>
    void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

    /// <summary>
    /// 设置对话框中"确定"按钮的文本
    /// </summary>
    /// <param name="pszText">要显示在按钮上的文本</param>
    void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

    /// <summary>
    /// 设置文件名编辑框旁边的标签文本
    /// </summary>
    /// <param name="pszLabel">要显示的标签文本</param>
    void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

    /// <summary>
    /// 获取用户选择的结果（适用于单选模式）
    /// </summary>
    /// <param name="ppsi">输出参数，返回表示用户所选项目的IShellItem接口</param>
    void GetResult(out IntPtr ppsi);
    // void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    /// <summary>
    /// 向对话框的导航面板添加自定义位置
    /// </summary>
    /// <param name="psi">表示要添加的位置的IShellItem接口</param>
    /// <param name="alignment">指定位置的对齐方式（顶部或底部）</param>
    void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] FDAP alignment);

    /// <summary>
    /// 设置默认的文件扩展名
    /// </summary>
    /// <param name="pszDefaultExtension">默认扩展名（不带前导点）</param>
    void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

    /// <summary>
    /// 关闭对话框并释放相关资源
    /// </summary>
    /// <param name="hr">关闭对话框的HRESULT代码，通常使用操作结果代码</param>
    void Close([MarshalAs(UnmanagedType.Error)] int hr);

    /// <summary>
    /// 设置与对话框关联的客户端GUID，用于保存/恢复对话框状态
    /// </summary>
    /// <param name="guid">客户端唯一标识GUID</param>
    void SetClientGuid([In] ref Guid guid);

    /// <summary>
    /// 清除与客户端GUID关联的所有保存状态
    /// </summary>
    void ClearClientData();

    /// <summary>
    /// 设置自定义筛选器对象，用于高级文件筛选
    /// </summary>
    /// <param name="pFilter">实现IFileDialogCustomize接口的筛选器对象</param>
    void SetFilter([In, MarshalAs(UnmanagedType.Interface)] nint pFilter);

    /// <summary>
    /// 获取用户选择的所有结果（适用于多选模式）
    /// 官方文档：https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifiledialog-getresults
    /// </summary>
    /// <param name="ppenum">输出参数，返回包含所有所选项目的 IShellItemArray 接口</param>
    /// <returns>HRESULT 结果码（S_OK 表示成功，其他值表示失败）</returns>
    /// <remarks>
    /// 1. 仅当对话框以多选模式（FOS_ALLOWMULTISELECT）打开且用户点击"确定"（Show 返回 S_OK）时有效
    /// 2. 调用方必须负责释放返回的 IShellItemArray 对象（使用 Marshal.ReleaseComObject）
    /// 3. 若未选择任何文件，返回的 ppenum 可能为 null，需提前判断
    /// </remarks>
    void GetResults(out IShellItemArray ppenum);

    /// <summary>
    /// 获取对话框中当前选中的所有项目
    /// </summary>
    /// <param name="ppsai">输出参数，返回包含所有选中项目的IShellItemArray接口</param>
    void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);

    #endregion

}


