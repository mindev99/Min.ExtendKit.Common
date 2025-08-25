using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Core.Interfaces.Dialogs;

/// <summary>
/// 表示一个 <see cref="IShellItem"/> 对象的集合，用于处理多个文件系统对象 。  
/// 通常由 <see cref="IFileOpenDialog.GetResults"/> 返回，用于多选文件的情况。
/// </summary>
/// <remarks>
/// <code>接口GUID: b63ea76d-1f85-456f-a19c-48159efa858b</code>
/// 官方文档: https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nn-shobjidl_core-ishellitemarray
/// </remarks>
[ComImport]
[Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItemArray
{
    /// <summary>
    /// 为数组中的某一项绑定到指定的处理程序（Handler）。
    /// </summary>
    /// <param name="pbc">可选的绑定上下文，通常传 <see cref="IntPtr.Zero"/>。</param>
    /// <param name="bhid">要绑定的处理程序 ID（例如 BHID_Stream, BHID_SFObject）。</param>
    /// <param name="riid">请求的接口 IID。</param>
    /// <param name="ppvOut">调用成功时，返回对应的接口指针。</param>
    void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);

    /// <summary>
    /// 获取数组中所有项的属性存储（Property Store）。
    /// </summary>
    /// <param name="flags">属性存储标志。</param>
    /// <param name="riid">请求的接口 IID（通常为 IPropertyStore）。</param>
    /// <param name="ppv">调用成功时，返回接口指针。</param>
    void GetPropertyStore(int flags, ref Guid riid, out IntPtr ppv);

    /// <summary>
    /// 根据指定的属性键类型，获取属性描述列表。
    /// </summary>
    /// <param name="keyType">属性键类型的 <see cref="Guid"/>。</param>
    /// <param name="riid">请求的接口 IID。</param>
    /// <param name="ppv">调用成功时，返回接口指针。</param>
    void GetPropertyDescriptionList(ref Guid keyType, ref Guid riid, out IntPtr ppv);

    /// <summary>
    /// 获取数组中所有项的综合属性标志。
    /// </summary>
    /// <param name="dwAttribFlags">要查询的属性标志。</param>
    /// <param name="sfgaoMask">属性掩码，指定要检索哪些属性。</param>
    /// <param name="psfgaoAttribs">调用成功时，返回属性标志。</param>
    void GetAttributes(uint dwAttribFlags, uint sfgaoMask, out uint psfgaoAttribs);

    /// <summary>
    /// 获取数组中项的数量。
    /// </summary>
    /// <param name="pdwNumItems">调用成功时，返回项的数量。</param>
    void GetCount(out uint pdwNumItems);

    /// <summary>
    /// 根据索引获取数组中的某一项。
    /// </summary>
    /// <param name="dwIndex">基于零的索引。</param>
    /// <param name="ppsi">调用成功时，返回 <see cref="IShellItem"/> 对象。</param>
    void GetItemAt(uint dwIndex, out IShellItem ppsi);

    /// <summary>
    /// 获取数组项的枚举器。
    /// </summary>
    /// <param name="ppenumShellItems">调用成功时，返回 <see cref="IEnumShellItems"/> 枚举接口。</param>
    void EnumItems(out IEnumShellItems ppenumShellItems);
}

