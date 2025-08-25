using System.Runtime.InteropServices;

using Min.ExtendKit.Common.Core.Struct;
using Min.ExtendKit.Common.Core.Win32.API;

namespace Min.ExtendKit.Common.Dialogs;

/// <summary>
/// Win32 颜色选择器封装类，提供在 Windows 系统中显示颜色选择对话框的方法。
/// </summary>
/// <remarks>
/// 1. 内部调用 Win32 API <c>ChooseColor</c> 显示系统颜色选择对话框。  
/// 2. 返回值使用自定义 <see cref="MinColor"/> 结构体，跨 WPF、WinForms 或控制台程序可用。  
/// 3. 透明度固定为 255，因为 Win32 原生颜色选择对话框不支持 Alpha 通道。  
/// 4. 支持父窗口句柄，可在 WPF 或 WinForms 中模态显示。  
/// 5. 支持自定义初始颜色和自定义颜色数组。  
/// 6. 用户取消选择时返回 <c>null</c>。
/// </remarks>
public static class ColorDialog
{
    /// <summary>
    /// 初始化对话框时使用 <see cref="CHOOSECOLOR.rgbResult"/> 指定的颜色。
    /// 如果不设置此标志，颜色选择对话框会使用默认颜色（通常为黑色）。
    /// </summary>
    public const uint CC_RGBINIT = 0x00000001;

    /// <summary>
    /// 打开对话框时显示完整的颜色调色板，而不仅仅是基本颜色。
    /// 同时允许显示“自定义颜色”区。
    /// </summary>
    public const uint CC_FULLOPEN = 0x00000002;

    /// <summary>
    /// 显示 Win32 原生颜色选择对话框，并返回用户选择的颜色。
    /// </summary>
    /// <param name="winHandle">父窗口句柄，用于设置对话框的模态窗口。如果传入 IntPtr.Zero，则没有父窗口。</param>
    /// <returns>
    /// 返回用户选择的 <see cref="System.Windows.Media.Color"/>，如果用户取消选择，则返回 <c>null</c>。
    /// </returns>
    /// <remarks>
    /// 1. 调用本方法会弹出 Windows 系统原生颜色选择对话框（经典风格）。
    /// 2. 可通过 <see cref="CHOOSECOLOR"/> 的 <c>lpCustColors</c> 设置自定义颜色。
    /// 3. 通过 <c>hwndOwner</c> 设置父窗口，使对话框模态化并且关闭后焦点回到父窗口。
    /// 4. 颜色值默认初始为白色 (0xFFFFFF)。
    /// 5. 返回值需要根据 <see cref="System.Drawing.Color"/> RGB 转换为 <see cref="System.Windows.Media.Color"/>。
    /// </remarks>
    public static System.Drawing.Color? Show(nint? winHandle = null)
    {
        CHOOSECOLOR cc = new();
        cc.lStructSize = Marshal.SizeOf(cc);
        cc.Flags = CC_RGBINIT | CC_FULLOPEN;

        // 设置父窗口
        cc.hwndOwner = winHandle ?? IntPtr.Zero;

        uint[] customColors = new uint[16]; // 自定义颜色
        GCHandle handle = GCHandle.Alloc(customColors, GCHandleType.Pinned);
        cc.lpCustColors = handle.AddrOfPinnedObject();
        cc.rgbResult = 0xFFFFFF; // 默认白色

        bool result = DialogAPI.ChooseColor(ref cc);

        handle.Free();

        if (result)
        {
            byte r = (byte)(cc.rgbResult & 0xFF);
            byte g = (byte)((cc.rgbResult >> 8) & 0xFF);
            byte b = (byte)((cc.rgbResult >> 16) & 0xFF);
            return System.Drawing.Color.FromArgb(r, g, b);
        }
        return null;
    }

    /// <summary>
    /// 显示 Win32 系统颜色选择对话框，返回自定义 MinColor（含透明度：Alpha 默认 255（不透明），Win32 ChooseColor 不支持 Alpha）
    /// </summary>
    /// <param name="winHandle">父窗口句柄，模态显示。如果没有父窗口，传 IntPtr.Zero</param>
    /// <param name="initialColor">初始显示的颜色</param>
    /// <returns>用户选择的颜色，如果取消则返回 null</returns>
    public static MinColor? Show(nint? winHandle, MinColor? initialColor = null)
    {
        CHOOSECOLOR cc = new();
        cc.lStructSize = Marshal.SizeOf(cc);
        cc.Flags = CC_RGBINIT | CC_FULLOPEN;

        // 设置父窗口
        cc.hwndOwner = winHandle ?? IntPtr.Zero;

        uint[] customColors = new uint[16];
        GCHandle handle = GCHandle.Alloc(customColors, GCHandleType.Pinned);
        cc.lpCustColors = handle.AddrOfPinnedObject();

        // 设置初始颜色
        MinColor startColor = initialColor ?? new MinColor(255, 255, 255, 255); // 默认白色
        cc.rgbResult = (uint)((startColor.B << 16) | (startColor.G << 8) | startColor.R);

        bool result = DialogAPI.ChooseColor(ref cc);
        handle.Free();

        if (result)
        {
            byte r = (byte)(cc.rgbResult & 0xFF);
            byte g = (byte)((cc.rgbResult >> 8) & 0xFF);
            byte b = (byte)((cc.rgbResult >> 16) & 0xFF);
            // Alpha 默认 255（不透明），Win32 ChooseColor 不支持 Alpha
            return new MinColor(r, g, b, 255);
        }
        return null;
    }

}
