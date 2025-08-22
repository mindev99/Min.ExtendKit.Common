using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Core.Interfaces.Dialogs;

/// <summary>
/// 表示Shell项目数组的COM接口，用于处理多个文件系统对象
/// </summary>
/// <remarks>接口GUID: b63ea76d-1f85-456f-a19c-48159efa858b</remarks>
[ComImport]
[Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItemArray
{
    /// <summary>
    /// 获取 IShellItemArray 中包含的项目数量
    /// </summary>
    /// <param name="pdwNumItems">输出参数，返回数组中项目的数量</param>
    //void GetCount(out uint pdwNumItems);

    /// <summary>
    /// 获取指定索引的项目
    /// </summary>
    /// <param name="dwIndex">项目索引</param>
    /// <param name="ppsi">输出参数，返回指定索引的IShellItem</param>
    //void GetItemAt(uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);


    void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);
    void GetPropertyStore(int flags, ref Guid riid, out IntPtr ppv);
    void GetAttributes(uint dwAttribFlags, uint sfgaoMask, out uint psfgaoAttribs);

    void GetCount(out uint pdwNumItems);

    void GetItemAt(uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
    void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
}

