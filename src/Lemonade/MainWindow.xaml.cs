using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Diagnostics;
using Microsoft.Win32;

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

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "pmxモデルファイル(*.pmx)|*.pmx";
            if (ofd.ShowDialog() != true)
                return;

            ofd.Filter = "vmdモーションファイル(*.vmd)|*.vmd";
            if (ofd.ShowDialog() == true)
            {
            }
            
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["hwnd"] = source.Handle.ToString();
            dic["name"] = "noname";
            SakuraFMO.Save(dic);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
