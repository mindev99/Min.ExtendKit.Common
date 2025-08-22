using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Min.ExtendKit.Common.Core.Enum;
using Min.ExtendKit.Common.Core.Interfaces.Dialogs;
using Min.ExtendKit.Common.Core.Win32.API;

namespace Min.ExtendKit.Common.Dialogs;

/// <summary>
/// 文件对话框封装类，提供现代 Windows 打开文件和保存文件功能。
/// 使用 IFileOpenDialog / IFileSaveDialog COM 接口实现。
/// </summary>

[SupportedOSPlatform("windows")]
public static class FileDialog
{
    /// <summary>
    /// 弹出“打开文件”对话框
    /// </summary>
    /// <param name="title">对话框标题</param>
    /// <param name="defaultPath">默认打开路径，可为 null</param>
    /// <param name="allowMultiSelect">是否允许多选</param>
    /// <param name="filter">文件过滤器，例如 "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*"</param>
    /// <param name="onError">异常回调，可处理错误信息（例如弹窗或日志记录），可为 null</param>
    /// <param name="winHandle">父窗口句柄，可为 null</param>
    /// <returns>用户选择的文件路径列表，如果取消选择返回 null</returns>
    [SupportedOSPlatform("windows")]
    public static string[]? OpenFile(string title, string? defaultPath = null, bool allowMultiSelect = false, COMDLG_FILTERSPEC[]? filter = null, Action<string, string?>? onError = null, nint? winHandle = null)
    {
        IFileOpenDialog? dialog = null;
        IntPtr shellItemPtr = IntPtr.Zero;

        try
        {
            // 创建 IFileOpenDialog COM 实例
            Type? dialogType = Type.GetTypeFromCLSID(new Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7"));
            if (dialogType == null)
            {
                onError?.Invoke("无法获取 IFileOpenDialog 类型", null);
                return null;
            }

            dialog = Activator.CreateInstance(dialogType) as IFileOpenDialog;
            if (dialog == null)
            {
                onError?.Invoke("无法创建 IFileOpenDialog 实例", null);
                return null;
            }

            // 安全设置选项
            dialog.GetOptions(out FOS currentOptions);

            // 设置 FOS 选项： 只返回文件系统路径 | 文件必须存在 | 路径必须存在 | 打开后不改变当前工作目录
            var fosData = currentOptions | FOS.FOS_FORCEFILESYSTEM | FOS.FOS_FILEMUSTEXIST | FOS.FOS_PATHMUSTEXIST | FOS.FOS_NOCHANGEDIR;

            // 如果允许多选，则添加 FOS_ALLOWMULTISELECT 选项
            if (allowMultiSelect)
            {
                fosData |= FOS.FOS_ALLOWMULTISELECT;
            }

            // 设置对话框选项
            dialog.SetOptions(fosData);

            // 设置标题
            dialog.SetTitle(title);


            // 设置文件过滤器
            if (filter != null && filter.Length > 0)
            {
                // 使用 COMDLG_FILTERSPEC 数组直接传给 COM
                dialog.SetFileTypes((uint)filter.Length, filter);

                // 默认选中第一个过滤器
                dialog.SetFileTypeIndex(1);
            }

            // 如果提供默认路径，则设置为初始文件夹
            if (!string.IsNullOrEmpty(defaultPath) && System.IO.Directory.Exists(defaultPath))
            {
                IntPtr pidl = IntPtr.Zero;
                try
                {
                    // 使用 Win32 API 创建 IShellItem 指针
                    SHCreateItemFromParsingName(defaultPath, IntPtr.Zero, typeof(IShellItem).GUID, out pidl);
                    dialog.SetFolder(pidl);
                }
                finally
                {
                    if (pidl != IntPtr.Zero)
                        Marshal.Release(pidl);
                }
            }

            // 显示对话框，IntPtr.Zero 表示无父窗口，使用传入的窗口句柄或默认值
            int hr = dialog.Show(winHandle ?? IntPtr.Zero);

            // 用户取消或错误
            if (hr != 0) return null;

            // 是否开启多选
            if (allowMultiSelect)
            {
                // 多文件处理
                dialog.GetResults(out IShellItemArray resultsArray);
                if (resultsArray == null)
                    return null;

                try
                {
                    resultsArray.GetCount(out uint count);
                    var paths = new List<string>((int)count);

                    for (uint i = 0; i < count; i++)
                    {
                        resultsArray.GetItemAt(i, out IShellItem? item);
                        if (item == null)
                            continue;

                        try
                        {
                            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string? path);
                            if (!string.IsNullOrEmpty(path))
                            {
                                paths.Add(path);
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(item);
                        }
                    }

                    return paths.ToArray();
                }
                finally
                {
                    Marshal.ReleaseComObject(resultsArray);
                }
            }
            else
            {
                /** 选择单文件模式 **/

                dialog.GetResult(out IntPtr resultPtr);
                if (resultPtr == IntPtr.Zero) return null;

                // 获取 IShellItem 接口
                Guid shellItemGuid = typeof(IShellItem).GUID;
                Marshal.QueryInterface(resultPtr, ref shellItemGuid, out IntPtr itemPtr);

                var shellItem = (IShellItem)Marshal.GetObjectForIUnknown(itemPtr);

                // 获取文件路径
                shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string path);

                // 打印调试
                Debug.WriteLine($"单文件：{path}");

                // 释放 COM 对象
                Marshal.ReleaseComObject(shellItem);
                Marshal.Release(itemPtr);
                Marshal.Release(resultPtr);

                return [path];
            }
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message, ex.StackTrace);

            Debug.WriteLine("Message：" + ex.Message);
        }
        finally
        {
            // 释放 COM 对象，防止内存泄漏
            if (shellItemPtr != IntPtr.Zero)
            {
                Marshal.Release(shellItemPtr);
            }

            if (dialog != null)
            {
                _ = Marshal.ReleaseComObject(dialog);
            }
        }

        return null;
    }

    /// <summary>
    /// 弹出“保存文件”对话框
    /// </summary>
    /// <param name="title">对话框标题</param>
    /// <param name="defaultPath">默认保存路径，可为 null</param>
    /// <param name="defaultFileName">默认文件名，可为 null</param>
    /// <param name="filter">文件过滤器，例如 "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*"</param>
    /// <param name="onError">异常回调，可处理错误信息（例如弹窗或日志记录），可为 null</param>
    /// <param name="winHandle">父窗口句柄，可为 null</param>
    /// <returns>用户选择的保存文件路径，如果取消选择返回 null</returns>
    [SupportedOSPlatform("windows")]
    public static string? SaveFile(string title, string? defaultPath = null, string? defaultFileName = null, string? filter = null, Action<string, string?>? onError = null, nint? winHandle = null)
    {
        // TODO: 使用 IFileSaveDialog 实现
        // 设置 FOS 文件对话框选项：FOS_OVERWRITEPROMPT, FOS_PATHMUSTEXIST 等
        // 设置标题、默认路径、默认文件名、过滤器
        // 显示对话框 -> 获取选择的保存文件路径
        // 异常回调 -> onError
        throw new NotImplementedException();
    }


    [SupportedOSPlatform("windows")]
    private static void SHCreateItemFromParsingName(string path, IntPtr pbc, Guid riid, out IntPtr ppv)
    {
        API.SHCreateItemFromParsingName(path, pbc, ref riid, out ppv);
    }


}
