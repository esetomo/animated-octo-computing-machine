﻿using DxLibDLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

namespace Lemonade
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private int drawScreenHandle = -1;
        private int softImageHandle = -1;
        private int modelHandle = -1;
        private float angle;
        private WriteableBitmap bmp = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = new HwndSource(0, 0, 0, 0, 0, "DxLib", IntPtr.Zero);
            DX.SetUserWindow(source.Handle);

            DX.SetUseBackBufferTransColorFlag(DX.TRUE);

            if (DX.DxLib_Init() == -1)
                throw new Exception("DxLib Init failed");

            string[] files = Directory.GetFiles(".", "*.pmd", SearchOption.AllDirectories);
            modelHandle = DX.MV1LoadModel(files[0]);
            DX.MV1SetPosition(modelHandle, DX.VGet(250.0f, 170.0f, -260.0f));
            DX.MV1SetScale(modelHandle, DX.VGet(8.0f, 8.0f, 8.0f));
            angle = 0.0f;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            int w = (int)Width;
            int h = (int)Height;

            if (bmp == null)
            {
                drawScreenHandle = DX.MakeScreen(w, h, DX.TRUE);
                softImageHandle = DX.MakeSoftImage(w, h);
                bmp = new WriteableBitmap(w, h, 96.0, 96.0, PixelFormats.Bgra32, null);
                image1.Source = bmp;
            }

            DX.SetDrawScreen(drawScreenHandle);
            DX.ClearDrawScreen();
            RenderContent();
            DX.GetDrawScreenSoftImage(0, 0, (int)Width, (int)Height, softImageHandle);

            bmp.Lock();
            Int32Rect rect = new Int32Rect(0, 0, w, h);
            IntPtr buf = dx_GetImageAddressSoftImage(softImageHandle);
            bmp.WritePixels(rect, buf, w * h * 4, w * 4);
            bmp.AddDirtyRect(rect);
            bmp.Unlock();
        }

        // DxLibDotNet.dllにある宣言はvoid *を使っておりunsafeのため、ポインタを使わないものを代わりに宣言
        [DllImport("DxLib.dll")]
        extern static IntPtr dx_GetImageAddressSoftImage(int SIHandle);

        private void RenderContent()
        {
            angle += 0.1f;
            DX.MV1SetRotationXYZ(modelHandle, DX.VGet(0.0f, angle, 0.0f));
            DX.MV1DrawModel(modelHandle);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DX.DxLib_End();            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
