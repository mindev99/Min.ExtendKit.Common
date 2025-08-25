using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Core.Struct;

/// <summary>
/// 封装 Win32 API <c>BROWSEINFO</c> 结构体，
/// 用于配置 <c>SHBrowseForFolder</c> 文件夹选择对话框的参数。
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct BROWSEINFO
{
    /// <summary>
    /// 父窗口句柄。指定对话框的宿主窗口。
    /// </summary>
    internal IntPtr hwndOwner;

    /// <summary>
    /// 根文件夹的项标识符 (PIDL)。
    /// 对话框中显示的浏览范围将限定在此文件夹及其子文件夹下。
    /// 常用值为 <c>IntPtr.Zero</c>（表示桌面）。
    /// </summary>
    internal IntPtr pidlRoot;

    /// <summary>
    /// 接收用户选择的文件夹显示名称的缓冲区。
    /// 必须是一个长度为 MAX_PATH 的字符串缓冲。
    /// </summary>
    internal string pszDisplayName;

    /// <summary>
    /// 对话框标题，在对话框顶部显示。
    /// </summary>
    internal string lpszTitle;

    /// <summary>
    /// 对话框样式标志。由 <c>BIF_XXX</c> 常量组合。
    /// 例如 <c>BIF_RETURNONLYFSDIRS</c>, <c>BIF_NEWDIALOGSTYLE</c> 等。
    /// </summary>
    internal uint ulFlags;

    /// <summary>
    /// 回调函数指针。用于接收对话框事件（例如初始化、选择更改）。
    /// 如果不需要，可以为 <c>IntPtr.Zero</c>。
    /// </summary>
    internal IntPtr lpfn;

    /// <summary>
    /// 传递给回调函数的自定义参数。
    /// 可用于在回调中访问调用方上下文。
    /// </summary>
    internal IntPtr lParam;

    /// <summary>
    /// 接收系统图像列表中选定文件夹的图标索引。
    /// </summary>
    internal int iImage;
}

/// <summary>
/// Win32 API 用于打开文件保存/打开对话框的结构体。
/// 对应 C++ 的 OPENFILENAMEW 结构体。
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct OPENFILENAME
{
    /// <summary>
    /// 结构体大小（以字节为单位）。必须设置为 sizeof(OPENFILENAME)。
    /// </summary>
    public int lStructSize;

    /// <summary>
    /// 所属窗口句柄，文件对话框会以此窗口为父窗口。
    /// </summary>
    public IntPtr hwndOwner;

    /// <summary>
    /// 保留，通常设置为 IntPtr.Zero。
    /// </summary>
    public IntPtr hInstance;

    /// <summary>
    /// 文件类型过滤器，格式为："文本文件 (*.txt)\0*.txt\0所有文件 (*.*)\0*.*\0"。
    /// 每个过滤器由显示名称 + "\0" + 通配符 + "\0" 组成，结尾需双 "\0"。
    /// </summary>
    public string lpstrFilter;

    /// <summary>
    /// 自定义过滤器缓冲区，通常为 null。
    /// </summary>
    public string lpstrCustomFilter;

    /// <summary>
    /// 自定义过滤器缓冲区大小（字符数）。
    /// </summary>
    public int nMaxCustFilter;

    /// <summary>
    /// 当前选择的过滤器索引，从 1 开始。
    /// </summary>
    public int nFilterIndex;

    /// <summary>
    /// 文件路径缓冲区，用于输入默认文件名和输出用户选择的文件。
    /// </summary>
    public string lpstrFile;

    /// <summary>
    /// 文件路径缓冲区大小（字符数）。
    /// </summary>
    public int nMaxFile;

    /// <summary>
    /// 文件标题缓冲区，通常不用，可设为 null。
    /// </summary>
    public string lpstrFileTitle;

    /// <summary>
    /// 文件标题缓冲区大小（字符数）。
    /// </summary>
    public int nMaxFileTitle;

    /// <summary>
    /// 对话框初始目录，可以为空。
    /// </summary>
    public string lpstrInitialDir;

    /// <summary>
    /// 对话框标题。
    /// </summary>
    public string lpstrTitle;

    /// <summary>
    /// 对话框标志，控制行为，如 OFN_OVERWRITEPROMPT, OFN_NOCHANGEDIR, OFN_PATHMUSTEXIST 等。
    /// </summary>
    public int Flags;

    /// <summary>
    /// 文件名在 lpstrFile 中的偏移量（输出）。
    /// </summary>
    public short nFileOffset;

    /// <summary>
    /// 文件扩展名在 lpstrFile 中的偏移量（输出）。
    /// </summary>
    public short nFileExtension;

    /// <summary>
    /// 默认扩展名，例如 "txt"。
    /// 当用户没有输入扩展名时自动添加。
    /// </summary>
    public string lpstrDefExt;

    /// <summary>
    /// 用户自定义数据，通常为 IntPtr.Zero。
    /// </summary>
    public IntPtr lCustData;

    /// <summary>
    /// 对话框钩子函数指针，通常为 IntPtr.Zero。
    /// </summary>
    public IntPtr lpfnHook;

    /// <summary>
    /// 对话框模板名称，通常为 null。
    /// </summary>
    public string lpTemplateName;

    /// <summary>
    /// 保留字段，必须为 IntPtr.Zero。
    /// </summary>
    public IntPtr pvReserved;

    /// <summary>
    /// 保留字段，必须为 0。
    /// </summary>
    public int dwReserved;

    /// <summary>
    /// 扩展标志，通常为 0。
    /// </summary>
    public int flagsEx;
}
