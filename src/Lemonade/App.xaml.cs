using DxLibDLL;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Lemonade
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SakuraFMO.Open();

            // ダミーのウィンドウハンドルを設定することによりDxLib側でウィンドウが生成されないようにする。
            HwndSource dummySource = new HwndSource(0, 0, 0, 0, 0, "DxLib", IntPtr.Zero);
            DX.SetUserWindow(dummySource.Handle);

            DX.SetUseBackBufferTransColorFlag(DX.TRUE);

            if (DX.DxLib_Init() == -1)
                throw new Exception("DxLib Init failed");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SakuraFMO.Close();

            DX.DxLib_End();
        }
    }
}
