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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MMF.CG.Model.MMD;
using Microsoft.Win32;
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
