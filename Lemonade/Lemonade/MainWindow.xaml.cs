using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Diagnostics;
using Microsoft.Win32;
using MMF.CG.Model;
using MMF.CG.Model.MMD;
using MMF.CG.Motion;
using MMDFileParser.PMXModelParser;

namespace Lemonade
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {        
        private MMDModel model;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(this);
            if (Application.Current.MainWindow == this)
                source.AddHook(new HwndSourceHook(SakuraAPI.WndProc));

            // remove basic grid
            if(renderControl.WorldSpace.DrawableResources.Count > 0)
                renderControl.WorldSpace.RemoveResource(renderControl.WorldSpace.DrawableResources[0]);

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

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["hwnd"] = source.Handle.ToString();
            dic["name"] = model.Model.ModelInfo.ModelName_En;
            SakuraFMO.Save(dic);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
