using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using Min.ExtendKit.Common.Dialogs;

namespace Test.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        //string? t = string.Empty;
        //t = FolderDialog.ModernBrowseForFolder("[Modern] 选择文件夹", null, onError: (msg, ex) =>
        //{
        //    MessageBox.Show($"{msg}\n{ex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        //},new WindowInteropHelper(this).Handle);

        //t = FolderDialog.ClassicBrowseForFolder("[Classic] 选择文件夹", onError: (msg, ex) =>
        //{
        //    MessageBox.Show($"{msg}\n{ex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        //}, new WindowInteropHelper(this).Handle);

        //var filters = new COMDLG_FILTERSPEC[]
        //{
        //    new() { pszName = "文本文件 (*.txt)", pszSpec = "*.txt" },
        //    new() { pszName = "所有文件 (*.*)",   pszSpec = "*.*"  }
        //};
        //string[]? array = Min.ExtendKit.Common.Dialogs.FileDialog.OpenFile("打开文件", defaultPath: @"C:\", allowMultiSelect: true, filter: filters, winHandle: new WindowInteropHelper(this).Handle);
        //if (array != null)
        //{
        //    MessageBox.Show(string.Join(Environment.NewLine, array));
        //}

        //string? path = Min.ExtendKit.Common.Dialogs.FileDialog.SaveFile(title: "请选择保存位置", defaultPath: @"C:\", defaultFileName: "test.txt", filter: filters, onError: (msg, stack) => Console.WriteLine($"错误: {msg} - {stack}"), winHandle: new WindowInteropHelper(this).Handle);

        //MessageBox.Show(path);

        MinColor? selectedcolor = ColorDialog.Show(new WindowInteropHelper(this).Handle, new MinColor(255, 0, 0));
        if (selectedcolor.HasValue)
        {
            MinColor color = selectedcolor.Value;
            MessageBox.Show($"选择的颜色: R={color.R}, G={color.G}, B={color.B}, A={color.A}");
        }

    }



}