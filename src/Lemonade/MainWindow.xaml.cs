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
        private WriteableBitmap bmp = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(this);
            if (Application.Current.MainWindow == this)
                source.AddHook(new HwndSourceHook(SakuraAPI.WndProc));

            // ダミーのウィンドウハンドルを設定することによりDxLib側でウィンドウが生成されないようにする。
            HwndSource dummySource = new HwndSource(0, 0, 0, 0, 0, "DxLib", IntPtr.Zero);
            DX.SetUserWindow(dummySource.Handle);

            DX.SetUseBackBufferTransColorFlag(DX.TRUE);

            if (DX.DxLib_Init() == -1)
                throw new Exception("DxLib Init failed");

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "pmdモデルファイル(*.pmd)|*.pmd";
            if (ofd.ShowDialog() != true)
                return;

            modelHandle = DX.MV1LoadModel(ofd.FileName);            

            /*
            ofd.Filter = "vmdモーションファイル(*.vmd)|*.vmd";
            if (ofd.ShowDialog() == true)
            {
            }
             */
            
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["hwnd"] = source.Handle.ToString();
            dic["name"] = "noname";
            SakuraFMO.Save(dic);

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

        [DllImport("DxLib.dll")]
        extern static IntPtr dx_GetImageAddressSoftImage(int SIHandle);

        private void RenderContent()
        {
            DX.SetupCamera_Perspective(0.25f);
            DX.VECTOR position = DX.VGet(0.0f, 18.0f, -100.0f);
            DX.VECTOR target = DX.VGet(0.0f, 10.0f, 0.0f);
            DX.SetCameraPositionAndTarget_UpVecY(position, target);

            DX.MV1SetPosition(modelHandle, DX.VGet(0.0f, 0.0f, 0.0f));
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
