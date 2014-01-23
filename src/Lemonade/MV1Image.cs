using DxLibDLL;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lemonade
{
    class MV1Image : Image
    {
        private int drawScreenHandle = -1;
        private int softImageHandle = -1;
        private int modelHandle = -1;
        private int attachIndex = -1;
        private WriteableBitmap bmp = null;
        private Stopwatch stopwatch;
        private double prevSec = 0.0;
        private int currentFrame = 0;
        private int totalFrame = 0;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Control parentControl = this.Parent as Control;
            this.Width = parentControl.Width;
            this.Height = parentControl.Height;

            modelHandle = DX.MV1LoadModel(@"dat\sakura.pmd");
            attachIndex = DX.MV1AttachAnim(modelHandle, 0);
            totalFrame = (int)DX.MV1GetAnimTotalTime(modelHandle, attachIndex);

            stopwatch = new Stopwatch();
            stopwatch.Start();

            ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
        }

        void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
        {
            WaitNextFrame();

            int w = (int)Width;
            int h = (int)Height;

            if (bmp == null)
            {
                drawScreenHandle = DX.MakeScreen(w, h, DX.TRUE);
                softImageHandle = DX.MakeSoftImage(w, h);
                bmp = new WriteableBitmap(w, h, 96.0, 96.0, PixelFormats.Bgra32, null);
                this.Source = bmp;
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

        private void WaitNextFrame()
        {
            double frameSec = 1.0 / 30.0;
            double nowSec = stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
            double nextSec = prevSec + frameSec;
            double waitSec = nextSec - nowSec;

            if (waitSec > 0.0)
                Thread.Sleep((int)(waitSec * 1000));

            prevSec = Math.Max(nowSec, nextSec);
        }

        [DllImport("DxLib.dll")]
        extern static IntPtr dx_GetImageAddressSoftImage(int SIHandle);

        private void RenderContent()
        {
            DX.SetupCamera_Perspective(0.25f);
            DX.VECTOR position = DX.VGet(0.0f, 18.0f, -60.0f);
            DX.VECTOR target = DX.VGet(0.0f, 10.0f, 0.0f);
            DX.SetCameraPositionAndTarget_UpVecY(position, target);

            DX.MV1SetPosition(modelHandle, DX.VGet(0.0f, 0.0f, 0.0f));

            currentFrame++;
            if (currentFrame >= totalFrame)
                currentFrame = 0;
            int r = DX.MV1SetAttachAnimTime(modelHandle, attachIndex, currentFrame);

            DX.MV1DrawModel(modelHandle);

            DX.DrawString(400, 450, string.Format("{0}", currentFrame), DX.GetColor(0, 255, 0));
        }
    }
}
