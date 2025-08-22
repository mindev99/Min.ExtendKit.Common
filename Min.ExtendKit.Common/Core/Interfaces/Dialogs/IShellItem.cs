using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Min.ExtendKit.Common.Core.Enum;

namespace Min.ExtendKit.Common.Core.Interfaces.Dialogs;

/// <summary>
/// IShellItem COM 接口，用于获取文件或文件夹路径
/// </summary>
[ComImport]
[Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItem
{
    void BindToHandler([In] nint pbc, [In] ref Guid bhid, [In] ref Guid riid, out nint ppv);

    void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

    void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

    void Compare([In] IShellItem psi, [In] uint hint, out int piOrder);
}

