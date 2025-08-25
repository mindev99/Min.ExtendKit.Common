using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using Min.ExtendKit.Common.Core.Enum;
using Min.ExtendKit.Common.Core.Interfaces.Dialogs;
using Min.ExtendKit.Common.Core.Struct;
using Min.ExtendKit.Common.Core.Win32.API;
using Min.ExtendKit.Common.Core.Win32.COM;

using static Min.ExtendKit.Common.Core.Win32.COM.FileDialogCOM;

namespace Min.ExtendKit.Common.Dialogs;

/// <summary>
/// Windows 文件夹选择对话框。
/// <code>文件夹选择对话框封装类，提供现代和经典两种方式。</code>
/// </summary>

[SupportedOSPlatform("windows")]
public static class FolderDialog
{
    /** 使用 Win32 COM IFileOpenDialog + IShellItem 实现。 **/

    /// <summary>
    /// 浏览文件夹，弹出文件夹选择对话框
    /// </summary>
    /// <param name="title">对话框标题，由调用者自定义</param>
    /// <param name="defaultPath">默认打开路径，可为 null</param>
    /// <param name="onError">异常回调，可处理错误信息（例如弹窗或日志记录），可为 null</param>
    /// <param name="winHandle"> 父窗口句柄，可为 <c>null</c>。如果为 <c>null</c>，则对话框没有父窗口。
    /// <code>new WindowInteropHelper(Window).Handle</code>
    /// </param>
    /// <returns>用户选择的文件夹路径，如果取消选择则返回 null</returns>
    [SupportedOSPlatform("windows")]
    public static string? ModernBrowseForFolder(string title, string? defaultPath = null, Action<string, string?>? onError = null, nint? winHandle = null)
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
            // dialog = Activator.CreateInstance(dialogType) as FileDialogCOM.IFileOpenDialog;
            dialog = Activator.CreateInstance(dialogType) as IFileOpenDialog;
            if (dialog == null)
            {
                onError?.Invoke("无法创建 IFileOpenDialog 实例", null);
                return null;
            }

            // 设置对话框选项
            dialog.SetOptions(FOS.FOS_PICKFOLDERS |   // 选择文件夹
                              FOS.FOS_FORCEFILESYSTEM | // 只返回文件系统路径
                              FOS.FOS_PATHMUSTEXIST);  // 路径必须存在

            // 设置标题
            dialog.SetTitle(title);

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

            // S_OK
            if (hr == 0)
            {
                dialog.GetResult(out shellItemPtr);
                if (shellItemPtr != IntPtr.Zero)
                {
                    var shellItem = (IShellItem)Marshal.GetObjectForIUnknown(shellItemPtr);
                    shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string path);
                    return path;
                }
            }
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message, ex.StackTrace);
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
    /// 显示经典的 Windows 文件夹选择对话框 (基于 SHBrowseForFolder)。
    /// </summary>
    /// <param name="title">对话框标题</param>
    /// <param name="onError">错误回调（可用于显示消息或记录日志），可为 null</param>
    /// <param name="winHandle">父窗口句柄，默认为 null</param>
    /// <returns>选择的文件夹路径，用户取消时返回 null</returns>
    [SupportedOSPlatform("windows")]
    public static string? ClassicBrowseForFolder(string title, Action<string?, string?>? onError = null, nint? winHandle = null)
    {
        IntPtr pidl = IntPtr.Zero;
        IntPtr pidlRoot = IntPtr.Zero;

        try
        {
            // 获取桌面作为根目录
            if (DialogAPI.SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL_DESKTOP, out pidlRoot) != 0)
            {
                return null;
            }

            var browseInfo = new BROWSEINFO
            {
                hwndOwner = winHandle ?? IntPtr.Zero,
                pidlRoot = pidlRoot,
                lpszTitle = title,
                ulFlags = BIF_RETURNONLYFSDIRS | BIF_NEWDIALOGSTYLE | BIF_EDITBOX | BIF_STATUSTEXT
            };

            // 显示对话框
            pidl = DialogAPI.SHBrowseForFolder(ref browseInfo);

            if (pidl != IntPtr.Zero)
            {
                // 取路径
                IntPtr pathPtr = Marshal.AllocCoTaskMem(260 * 2 + 1); // MAX_PATH
                try
                {
                    if (DialogAPI.SHGetPathFromIDList(pidl, pathPtr))
                    {
                        string? path = Marshal.PtrToStringAuto(pathPtr);
                        return path;
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pathPtr);
                }
            }
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message, ex.StackTrace);
        }
        finally
        {
            if (pidl != IntPtr.Zero) DialogAPI.CoTaskMemFree(pidl);
            if (pidlRoot != IntPtr.Zero) DialogAPI.CoTaskMemFree(pidlRoot);
        }

        return null;
    }

    [SupportedOSPlatform("windows")]
    private static void SHCreateItemFromParsingName(string path, IntPtr pbc, Guid riid, out IntPtr ppv)
    {
        DialogAPI.SHCreateItemFromParsingName(path, pbc, ref riid, out ppv);
    }

}
