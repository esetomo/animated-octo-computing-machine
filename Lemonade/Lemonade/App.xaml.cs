using System.Windows;

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
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SakuraFMO.Close();
        }
    }
}
