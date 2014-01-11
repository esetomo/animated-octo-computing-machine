using DxLibDLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private int drawScreenHandle;
        private int softImageHandle;
        private int modelHandle;
        private float a;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(this);
            DX.SetUserWindow(source.Handle);

            if (DX.DxLib_Init() == -1)
                throw new Exception("DxLib Init failed");

            drawScreenHandle = DX.MakeScreen((int)Width, (int)Height);
            DX.SetDrawScreen(drawScreenHandle);

            softImageHandle = DX.MakeARGB8ColorSoftImage((int)Width, (int)Height);

            string[] files = Directory.GetFiles(".", "*.pmd", SearchOption.AllDirectories);
            modelHandle = DX.MV1LoadModel(files[0]);
            DX.MV1SetPosition(modelHandle, DX.VGet(250.0f, 170.0f, -260.0f));
            DX.MV1SetScale(modelHandle, DX.VGet(8.0f, 8.0f, 8.0f));
            a = 0.0f;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DX.ClearDrawScreen();
            RenderContent();

            DX.GraphFilter(drawScreenHandle, DX.DX_GRAPH_FILTER_PREMUL_ALPHA);
            DX.SetDrawScreen(drawScreenHandle);
            DX.GetDrawScreenSoftImage(0, 0, (int)Width, (int)Height, softImageHandle);
            DX.UpdateLayerdWindowForPremultipliedAlphaSoftImage(softImageHandle);
        }

        private void RenderContent()
        {
            a += 0.1f;
            DX.MV1SetRotationXYZ(modelHandle, DX.VGet(0.0f, a, 0.0f));
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
