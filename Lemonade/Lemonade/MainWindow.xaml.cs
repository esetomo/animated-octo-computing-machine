using DxLibDLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            DX.SetFontSize(48);
            DX.DrawString(0, 100, DateTime.Now.ToString(), DX.GetColor(0, 255, 0));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DX.DxLib_End();
        }
    }
}
