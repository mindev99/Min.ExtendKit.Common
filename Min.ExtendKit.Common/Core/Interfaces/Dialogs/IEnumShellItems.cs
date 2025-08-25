using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Min.ExtendKit.Common.Core.Interfaces.Dialogs;

/// <summary>
/// 用于枚举Shell项目的COM接口，提供遍历IShellItemArray中项目的功能
/// 官方文档：https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-ienumshellitems
/// </summary>
/// <remarks>
/// 接口GUID: 77f10cf0-3db5-4966-85a9-25505585dc92
/// 通常通过IShellItemArray.EnumItems方法获取实例
/// 适用于Windows Vista及以上系统
/// </remarks>
[ComImport]
[Guid("77f10cf0-3db5-4966-85a9-25505585dc92")]
//[Guid("000214F2-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IEnumShellItems
{
    /// <summary>
    /// 获取指定数量的项目
    /// </summary>
    /// <param name="celt">要获取的项目数量</param>
    /// <param name="rgelt">接收项目的数组</param>
    /// <param name="pceltFetched">实际获取的项目数量</param>
    /// <returns> 
    /// HRESULT结果码：
    /// - S_OK：成功获取所有请求的项目
    /// - S_FALSE：获取的项目数量少于请求数量（已到达末尾）
    /// - 其他值：失败
    /// </returns>
    [PreserveSig]
    int Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IShellItem rgelt, out uint pceltFetched);

    /// <summary>
    /// 跳过指定数量的项目
    /// </summary>
    /// <param name="celt">要跳过的项目数量</param>
    /// <returns>
    /// HRESULT结果码：
    /// - S_OK：成功跳过指定数量的项目
    /// - S_FALSE：跳过的项目数量超过剩余数量
    /// - 其他值：失败
    /// </returns>
    [PreserveSig]
    int Skip(uint celt);

    /// <summary>
    /// 重置枚举器到起始位置
    /// </summary>
    void Reset();

    /// <summary>
    /// 创建枚举器的副本
    /// </summary>
    /// <param name="ppenum">输出参数，返回新的枚举器</param>
    void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumShellItems ppenum);
}
