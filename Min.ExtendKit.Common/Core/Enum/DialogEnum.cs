using System.Runtime.InteropServices;

namespace Min.ExtendKit.Common.Core.Enum;

/// <summary>
/// 定义AddPlace方法中位置的对齐方式
/// </summary>
internal enum FDAP
{
    /// <summary>将位置添加到导航面板顶部，优先显示</summary>
    FDAP_TOP = 0,
    /// <summary>将位置添加到导航面板底部，在系统位置之后显示</summary>
    FDAP_BOTTOM = 1
}

/// <summary>
/// 文件/文件夹对话框选项。
/// <code>IFileDialog 对话框选项标志（FOS）。用于控制文件/文件夹对话框的行为。</code>
/// </summary>
[Flags]
internal enum FOS : uint
{
    /// <summary>当保存文件时，如果文件已存在则提示覆盖</summary>
    FOS_OVERWRITEPROMPT = 0x00000002,
    /// <summary>严格按照指定的文件类型筛选</summary>
    FOS_STRICTFILETYPES = 0x00000004,
    /// <summary>不改变当前工作目录</summary>
    FOS_NOCHANGEDIR = 0x00000008,
    /// <summary>对话框用于选择文件夹而非文件</summary>
    FOS_PICKFOLDERS = 0x00000020,
    /// <summary>只显示文件系统中的项目</summary>
    FOS_FORCEFILESYSTEM = 0x00000040,
    /// <summary>显示所有非存储项</summary>
    FOS_ALLNONSTORAGEITEMS = 0x00000080,
    /// <summary>不验证所选文件的有效性</summary>
    FOS_NOVALIDATE = 0x00000100,
    /// <summary>允许选择多个文件</summary>
    FOS_ALLOWMULTISELECT = 0x00000200,
    /// <summary>要求所选路径必须存在</summary>
    FOS_PATHMUSTEXIST = 0x00000800,
    /// <summary>要求所选文件必须存在</summary>
    FOS_FILEMUSTEXIST = 0x00001000,
    /// <summary>当文件不存在时提示创建</summary>
    FOS_CREATEPROMPT = 0x00002000,
    /// <summary>启用共享感知模式</summary>
    FOS_SHAREAWARE = 0x00004000,
    /// <summary>不返回只读文件</summary>
    FOS_NOREADONLYRETURN = 0x00008000,
    /// <summary>不测试文件创建能力</summary>
    FOS_NOTESTFILECREATE = 0x00010000,
    /// <summary>隐藏最近使用的位置</summary>
    FOS_HIDEMRUPLACES = 0x00020000,
    /// <summary>隐藏固定的位置</summary>
    FOS_HIDEPINNEDPLACES = 0x00040000,
    /// <summary>不解析快捷方式</summary>
    FOS_NODEREFERENCELINKS = 0x00100000,
    /// <summary>显示预览面板</summary>
    FOS_SHOWPREVIEW = 0x00200000,
    /// <summary>只返回文件系统绝对路径</summary>
    FOS_FORCESHOWHIDDEN = 0x10000000
}

/// <summary>
/// 获取 ShellItem 名称方式
/// </summary>
internal enum SIGDN : uint
{
    SIGDN_FILESYSPATH = 0x80058000
}

