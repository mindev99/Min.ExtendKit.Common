using System;
using System.Runtime.InteropServices;

using Min.ExtendKit.Common.Core.Interfaces;

namespace Min.ExtendKit.Common.Core.Win32.COM;

/// <summary>
/// 封装 Windows 现代文件/文件夹对话框所需 COM 接口
/// </summary>
internal class FileDialogCOM
{
    #region === 经典 Win32 API 封装 ===

    // 对话框样式常量
    internal const uint BIF_RETURNONLYFSDIRS = 0x0001;
    internal const uint BIF_NEWDIALOGSTYLE = 0x0040;
    internal const uint BIF_EDITBOX = 0x0010;
    internal const uint BIF_STATUSTEXT = 0x0004;

    // 特殊文件夹 CSIDL 常量
    internal const int CSIDL_DESKTOP = 0x0000;
    internal const int CSIDL_DRIVES = 0x0011;
    internal const int CSIDL_MYDOCUMENTS = 0x0005;

    #endregion

    #region === 现代 Win32 API 封装 ===

    #endregion



}

