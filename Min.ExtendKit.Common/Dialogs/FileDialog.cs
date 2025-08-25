using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Min.ExtendKit.Common.Core.Enum;
using Min.ExtendKit.Common.Core.Interfaces.Dialogs;
using Min.ExtendKit.Common.Core.Struct;
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
                // 多选模式
                dialog.GetResults(out IShellItemArray? resultsArray);
                if (resultsArray == null) return null;

                try
                {
                    resultsArray.GetCount(out uint count);

                    if (count == 0)
                        return null;

                    var paths = new List<string>((int)count);

                    for (uint i = 0; i < count; i++)
                    {
                        resultsArray.GetItemAt(i, out IShellItem item);
                        try
                        {
                            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string path);
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
                    return [.. paths];
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
    public static string? SaveFile(string title, string? defaultPath = null, string? defaultFileName = null, COMDLG_FILTERSPEC[]? filter = null, Action<string, string?>? onError = null, nint? winHandle = null)
    {
        // 初始化文件对话框结构（Win32 API要求的标准结构）
        var ofn = new OPENFILENAME
        {
            // 1. 结构大小：必须设置为当前结构的字节数，用于API识别结构版本
            lStructSize = Marshal.SizeOf<OPENFILENAME>(),

            // 2. 父窗口句柄：关联对话框到指定窗口，使对话框始终显示在父窗口上方
            //    对应方法参数：winHandle（若为null则使用IntPtr.Zero）
            hwndOwner = winHandle ?? IntPtr.Zero,

            // 3. 实例句柄：通常为null，用于加载自定义模板（此处未使用）
            hInstance = IntPtr.Zero,

            // 4. 文件过滤器：指定可选择的文件类型，格式为"描述\0扩展名\0描述\0扩展名\0"
            //    对应方法参数：filter（将COMDLG_FILTERSPEC转换为API要求的null分隔格式）
            lpstrFilter = ConvertFilterToNullSeparated(filter) + "\0",

            // 5. 自定义过滤器缓冲区：用于存储用户修改后的过滤器（此处未使用）
            lpstrCustomFilter = null,

            // 6. 自定义过滤器最大长度：与lpstrCustomFilter配合使用（此处未使用）
            nMaxCustFilter = 0,

            // 7. 默认过滤器索引：指定默认选中的过滤器（从1开始，此处默认选中第一个）
            nFilterIndex = 1,

            // 8. 文件名缓冲区：用于存储用户选择的文件路径，需预分配足够空间
            //    初始值：空字符填充的缓冲区，长度由nMaxFile指定
            lpstrFile = new string('\0', 2048),

            // 9. 文件名缓冲区最大长度：限制lpstrFile的最大字符数（含终止符）
            nMaxFile = 2048,

            // 10. 文件标题缓冲区：用于存储文件名（不含路径，此处未使用）
            lpstrFileTitle = null,

            // 11. 文件标题缓冲区最大长度：与lpstrFileTitle配合使用（此处未使用）
            nMaxFileTitle = 0,

            // 12. 初始目录：指定对话框打开时的默认路径
            //     对应方法参数：defaultPath（若为null则使用当前工作目录）
            lpstrInitialDir = defaultPath,

            // 13. 对话框标题：设置对话框标题栏文本
            //     对应方法参数：title
            lpstrTitle = title,

            // 14. 对话框标志：组合位标志控制对话框行为
            //     - OFN_PATHMUSTEXIST (0x00000002)：要求路径必须存在
            //     - OFN_OVERWRITEPROMPT (0x00000008)：覆盖文件时提示用户
            Flags = 0x00000002 | 0x00000008,

            // 15. 文件路径偏移量：自动填充，指示文件名在lpstrFile中的起始位置（无需手动设置）
            nFileOffset = 0,

            // 16. 文件扩展名偏移量：自动填充，指示扩展名在lpstrFile中的起始位置（无需手动设置）
            nFileExtension = 0,

            // 17. 默认扩展名：当用户未输入扩展名时自动添加的扩展名
            //     对应方法参数：filter（提取第一个过滤器的扩展名）
            lpstrDefExt = filter != null ? GetDefaultExtension(filter) : null,

            // 18. 自定义数据：传递给钩子函数的数据（此处未使用钩子，设为null）
            lCustData = IntPtr.Zero,

            // 19. 钩子函数指针：自定义对话框行为的回调函数（此处未使用，设为null）
            lpfnHook = IntPtr.Zero,

            // 20. 模板名称：自定义对话框模板（此处使用默认模板，设为null）
            lpTemplateName = null,

            // 21. 保留参数：供系统使用，必须设为null
            pvReserved = IntPtr.Zero,

            // 22. 保留参数：供系统使用，必须设为0
            dwReserved = 0,

            // 23. 扩展标志：扩展的行为控制标志（此处未使用，设为0）
            flagsEx = 0
        };

        // 设置默认文件名（若提供）
        // 对应方法参数：defaultFileName
        if (!string.IsNullOrEmpty(defaultFileName))
        {
            // 确保文件名长度不超过缓冲区限制
            int maxLength = ofn.nMaxFile - 1; // 预留一个字符给终止符
            if (defaultFileName.Length > maxLength)
            {
                defaultFileName = defaultFileName.Substring(0, maxLength);
            }
            // 填充默认文件名到缓冲区，剩余部分用空字符填充
            ofn.lpstrFile = defaultFileName + new string('\0', ofn.nMaxFile - defaultFileName.Length);
        }

        // 调用Win32 API显示保存文件对话框
        if (DialogAPI.GetSaveFileName(ref ofn))
        {
            // 去除字符串末尾的空字符，返回有效的文件路径
            return ofn.lpstrFile.TrimEnd('\0');
        }
        else
        {
            // 获取错误代码：0表示用户正常取消，其他值表示错误
            int errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
            {
                onError?.Invoke("保存文件失败", $"错误代码: {errorCode}（可参考Win32错误码表）");
            }
            return null;
        }
    }


    [SupportedOSPlatform("windows")]
    private static void SHCreateItemFromParsingName(string path, IntPtr pbc, Guid riid, out IntPtr ppv)
    {
        DialogAPI.SHCreateItemFromParsingName(path, pbc, ref riid, out ppv);
    }

    /// <summary>
    /// 将COMDLG_FILTERSPEC数组转换为Win32 API要求的null分隔格式
    /// </summary>
    /// <param name="filter">文件过滤器数组</param>
    /// <returns>null分隔的过滤器字符串（如"文本文件\0*.txt\0所有文件\0*.*"）</returns>
    private static string ConvertFilterToNullSeparated(COMDLG_FILTERSPEC[]? filter)
    {
        if (filter == null || filter.Length == 0)
        {
            // 默认过滤器
            return "All (*.*)\0*.*";
        }

        var filterParts = new List<string>();
        foreach (var spec in filter)
        {
            filterParts.Add(spec.pszName);   // 过滤器描述（如"文本文件"）
            filterParts.Add(spec.pszSpec);   // 过滤器规则（如"*.txt"）
        }
        return string.Join("\0", filterParts);
    }

    /// <summary>
    /// 从过滤器中提取默认扩展名
    /// </summary>
    /// <param name="filter">文件过滤器数组</param>
    /// <returns>默认扩展名（如"txt"）</returns>
    private static string? GetDefaultExtension(COMDLG_FILTERSPEC[] filter)
    {
        // 提取第一个过滤器的规则（如"*.txt" → "txt"）
        string firstSpec = filter[0].pszSpec;
        int starIndex = firstSpec.IndexOf('*');
        if (starIndex != -1 && starIndex < firstSpec.Length - 1)
        {
            return firstSpec.Substring(starIndex + 1).TrimStart('.');
        }
        return null;
    }


}
