using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Min.ExtendKit.Common.Core.Enum;

namespace Min.ExtendKit.Common.Core.Interfaces.Dialogs;

/// <summary>
/// 表示 Windows Shell 项接口 (IShellItem)，用于操作文件系统对象或 Shell 对象。
/// </summary>
[ComImport]
[Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItem
{
    /// <summary>
    /// 绑定到指定的处理程序（Handler）接口。
    /// </summary>
    /// <param name="pbc">指向绑定上下文（IBindCtx）的指针，一般为 IntPtr.Zero。</param>
    /// <param name="bhid">指定要绑定的处理程序的 GUID。</param>
    /// <param name="riid">请求的接口 ID（IID）。</param>
    /// <param name="ppv">返回的接口指针。</param>
    /// <remarks>
    /// 可用于获取流、存储或其他扩展接口。
    /// </remarks>
    void BindToHandler([In] nint pbc, [In] ref Guid bhid, [In] ref Guid riid, out nint ppv);

    /// <summary>
    /// 获取此 Shell 项的父项。
    /// </summary>
    /// <param name="ppsi">返回父 IShellItem 对象。</param>
    /// <remarks>
    /// 如果该项没有父项（如桌面根目录），则返回 null。
    /// </remarks>
    void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    /// <summary>
    /// 获取 Shell 项的显示名称。
    /// </summary>
    /// <param name="sigdnName">指定显示名称类型的枚举值（SIGDN）。</param>
    /// <param name="ppszName">输出显示名称字符串。</param>
    /// <remarks>
    /// SIGDN 枚举可以指定返回解析路径、URL、显示名称等不同格式。
    /// </remarks>
    void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

    /// <summary>
    /// 获取 Shell 项的属性标志。
    /// </summary>
    /// <param name="sfgaoMask">属性掩码，指定需要查询的属性位。</param>
    /// <param name="psfgaoAttribs">返回实际属性值。</param>
    void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

    /// <summary>
    /// 将当前项与另一个 Shell 项进行比较。
    /// </summary>
    /// <param name="psi">要比较的 IShellItem 对象。</param>
    /// <param name="hint">比较提示（保留，可传 0）。</param>
    /// <param name="piOrder">返回比较结果：负值表示小于，0 表示相等，正值表示大于。</param>
    void Compare([In] IShellItem psi, [In] uint hint, out int piOrder);
}
