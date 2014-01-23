using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Diagnostics;
using Microsoft.Win32;
using DxLibDLL;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lemonade
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(this);
            if (Application.Current.MainWindow == this)
                source.AddHook(new HwndSourceHook(SakuraAPI.WndProc));

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["hwnd"] = source.Handle.ToString();
            dic["name"] = "noname";
            SakuraFMO.Save(dic);

            Top = SystemParameters.WorkArea.Bottom - Height;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Top = SystemParameters.WorkArea.Bottom - Height;
        }
    }
}
