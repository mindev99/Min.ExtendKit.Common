using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Dialogs;

/// <summary>
/// 定义文件对话框的过滤器结构，用于指定显示名称和实际的过滤模式。
/// </summary>
/// <remarks>
/// 该结构体常用于 <see cref="IFileOpenDialog.SetFileTypes"/> 方法，
/// 用来告诉文件对话框在“文件类型”下拉列表中显示哪些选项。
/// 每个 <see cref="COMDLG_FILTERSPEC"/> 对应一行文件类型选项，
/// <see cref="pszName"/> 为显示在界面上的文字，<see cref="pszSpec"/> 为实际匹配的通配符。
/// </remarks>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct COMDLG_FILTERSPEC
{
    /// <summary>
    /// 在对话框“文件类型”下拉列表中显示的描述性名称。
    /// <para>例如：<c>"文本文件 (*.txt)"</c></para>
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszName;

    /// <summary>
    /// 对应的文件匹配模式（通配符表达式），用于筛选文件。
    /// <para>例如：<c>"*.txt"</c> 或 <c>"*.png;*.jpg"</c></para>
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszSpec;
}
