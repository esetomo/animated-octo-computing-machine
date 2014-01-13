using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MMF.CG.Model.MMD;
using MMF.CG.Motion;

namespace Lemonade
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MMDModel model;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "pmxモデルファイル(*.pmx)|*.pmx";
            if (ofd.ShowDialog() != true)
                return;

            model = MMDModelWithPhysics.OpenLoad(ofd.FileName, renderControl.RenderContext);

            ofd.Filter = "vmdモーションファイル(*.vmd)|*.vmd";
            if (ofd.ShowDialog() == true)
            {
                IMotionProvider motion = model.MotionManager.AddMotionFromFile(ofd.FileName, true);
                model.MotionManager.ApplyMotion(motion, 0, ActionAfterMotion.Replay);
            }
            
            renderControl.WorldSpace.AddResource(model);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
